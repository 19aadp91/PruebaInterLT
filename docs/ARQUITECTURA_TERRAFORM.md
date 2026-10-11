# Arquitectura de Infraestructura Cloud en AWS con Terraform

**Sistema:** Plataforma de Registro Académico y Selección de Materias  
**Herramienta de IaC:** HashiCorp Terraform ($\ge 1.5.0$) / Provider AWS ($\sim> 5.0$)  
**Proveedor Cloud:** Amazon Web Services (AWS)  
**Organización:** Inter Rapidísimo S.A.  
**Estado:** Producción / Infraestructura Modularizada y Validada  

---

## 1. Visión General y Filosofía de Infraestructura

La infraestructura en la nube para la solución está diseñada bajo la filosofía de **Infraestructura como Código (IaC)**, orientada a entornos productivos de alta seguridad y disponibilidad, erradicando prácticas manuales (**Cero ClickOps**):

* **Aislamiento Estricto de Red:** Topología de VPC dividida en tres niveles de subredes (públicas, privadas de cómputo y aisladas de datos) distribuidas en dos Zonas de Disponibilidad (**Multi-AZ**).
* **Cero Secretos en Texto Plano:** Credenciales de base de datos y llaves criptográficas administradas exclusivamente en tiempo de ejecución por **AWS Secrets Manager**, inyectadas de forma nativa a contenedores mediante variables de entorno de .NET.
* **Cómputo Serverless Inmutable:** La API backend se ejecuta en **AWS ECS Fargate**, eliminando la sobrecarga operativa de parches y gestión de instancias EC2.
* **Distribución Segura de Frontend:** Los artefactos de la SPA en Angular 21 se almacenan en un bucket privado de **Amazon S3** expuesto globalmente por **Amazon CloudFront** mediante **Origin Access Control (OAC)**, con soporte para enrutamiento del lado del cliente (*SPA client-side routing*).

---

## 2. Diagrama de Arquitectura de Red y Servicios (Mermaid)

```mermaid
graph TD
    Client([Usuario / Navegador Web])

    subgraph Edge_Distribution [Capa Perimetral y CDN]
        CF[Amazon CloudFront CDN\nHTTPS + OAC]
        S3[(Amazon S3 Private Bucket\nAssets Angular 21)]
    end

    subgraph AWS_VPC ["AWS VPC: 10.0.0.0/16 (Multi-AZ)"]
        subgraph Public_Subnets ["Subredes Públicas (10.0.1.0/24, 10.0.2.0/24)"]
            ALB[Application Load Balancer\nHTTP:80 / HTTPS:443]
            NAT[NAT Gateway\nSalida Egress]
            IGW[Internet Gateway]
        end

        subgraph Private_Compute ["Subredes Privadas Cómputo (10.0.11.0/24, 10.0.12.0/24)"]
            ECS1["ECS Fargate Task (AZ-a)\nAPI .NET 8 (Puerto 8080)"]
            ECS2["ECS Fargate Task (AZ-b)\nAPI .NET 8 (Puerto 8080)"]
        end

        subgraph Isolated_Database ["Subredes Aisladas BD (10.0.21.0/24, 10.0.22.0/24)"]
            RDS[(Amazon RDS SQL Server\nPuerto 1433 T-SQL\nCero Internet)]
        end
    end

    subgraph Support_Services [Servicios de Gestión y Seguridad AWS]
        SM[AWS Secrets Manager\n(DB Conn + JWT Key)]
        CW[Amazon CloudWatch\nLogs /ecs/...-api]
        ECR[Amazon ECR\nDocker Registry]
    end

    Client -->|1. Descarga HTML/JS/CSS| CF
    CF -->|Lectura OAC Segura| S3
    Client -->|2. Peticiones API REST| ALB

    ALB -->|Reenvío HTTP:8080| ECS1
    ALB -->|Reenvío HTTP:8080| ECS2

    ECS1 -->|T-SQL:1433| RDS
    ECS2 -->|T-SQL:1433| RDS

    ECS1 -.->|Egress vía NAT| ECR
    ECS1 -.->|Lectura en Boot| SM
    ECS1 -.->|Logs stdout/stderr| CW
```

---

## 3. Estructura del Proyecto de Infraestructura (`terraform/`)

La infraestructura se estructuró de forma modular para garantizar máxima mantenibilidad, separación de responsabilidades y reutilización:

```text
terraform/
├── main.tf                    # Orquestación raíz, Provider AWS y consumo de módulos
├── variables.tf               # Variables parametrizables de entrada (región, ambiente, secrets)
├── outputs.tf                 # Puntos de conexión públicos y privados
├── terraform.tfvars.example   # Plantilla de valores de ejemplo para despliegue
└── modules/
    ├── vpc/                   # Red principal, 6 subredes en 2 AZs, IGW, NAT Gateway y tablas de ruta
    │   ├── main.tf
    │   ├── variables.tf
    │   └── outputs.tf
    ├── security/              # Security Groups (ALB, ECS, RDS) y Roles IAM (Least Privilege)
    │   ├── main.tf
    │   ├── variables.tf
    │   └── outputs.tf
    ├── database/              # Amazon RDS SQL Server (gp3 cifrado) en subredes aisladas
    │   ├── main.tf
    │   ├── variables.tf
    │   └── outputs.tf
    ├── secrets/               # AWS Secrets Manager (cadenas de conexión y llaves JWT)
    │   ├── main.tf
    │   ├── variables.tf
    │   └── outputs.tf
    ├── ecs-fargate/           # Clúster ECS, ALB, Target Group (/healthz) y Servicio Fargate
    │   ├── main.tf
    │   ├── variables.tf
    │   └── outputs.tf
    └── frontend-s3-cf/        # Bucket S3 privado con OAC y distribución CloudFront para SPA
        ├── main.tf
        ├── variables.tf
        └── outputs.tf
```

---

## 4. Desglose Técnico de los Módulos de Terraform

### 4.1. Módulo `modules/vpc` (Red y Enrutamiento Multi-AZ)
* **Bloque CIDR de VPC:** `10.0.0.0/16` con soporte de nombres y resolución DNS habilitados.
* **Segmentación de Subredes:**
  1. **Subredes Públicas (`10.0.1.0/24`, `10.0.2.0/24`):**
     - Distribuidas en `us-east-1a` y `us-east-1b`.
     - Alojan el Application Load Balancer y el NAT Gateway.
     - Asocian tabla de ruta pública dirigida al Internet Gateway (`0.0.0.0/0 -> igw`).
  2. **Subredes Privadas de Cómputo (`10.0.11.0/24`, `10.0.12.0/24`):**
     - Alojan las tareas de AWS ECS Fargate.
     - Tráfico saliente enrutado hacia el NAT Gateway para permitir descarga de imágenes desde Amazon ECR y conexión a AWS Secrets Manager.
  3. **Subredes Aisladas de Base de Datos (`10.0.21.0/24`, `10.0.22.0/24`):**
     - Alojan la instancia de base de datos Amazon RDS SQL Server.
     - **Cero conectividad a internet:** Sin rutas hacia Internet Gateway ni NAT Gateway; únicamente enrutamiento local de VPC.

---

### 4.2. Módulo `modules/security` (Perímetro de Red y Roles IAM)
* **Security Groups:**
  * `sg_alb`:
    - **Ingreso:** Puertos 80 (HTTP) y 443 (HTTPS) desde `0.0.0.0/0`.
    - **Egreso:** Puerto 8080 restringido hacia el Security Group de las tareas ECS.
  * `sg_ecs`:
    - **Ingreso:** Puerto 8080 restringido únicamente desde `sg_alb`.
    - **Egreso:** Todo el tráfico permitido para acceder a RDS (puerto 1433) y servicios AWS vía NAT.
  * `sg_rds`:
    - **Ingreso:** Puerto 1433 (T-SQL) permitido **únicamente** desde `sg_ecs`.
    - **Egreso:** 0 reglas de egreso.
* **Roles IAM con Principio de Mínimo Privilegio:**
  * `ecs_execution_role`: Permite al motor de ECS descargar imágenes desde ECR, escribir métricas/logs en CloudWatch y leer específicamente los secretos requeridos en Secrets Manager.
  * `ecs_task_role`: Asignado a la aplicación en ejecución.

---

### 4.3. Módulo `modules/database` (Amazon RDS SQL Server)
* **Motor:** Microsoft SQL Server (`sqlserver-ex` para optimización de costos en MVP / compatible con `sqlserver-se`).
* **Subnet Group:** Creado exclusivamente sobre las subredes aisladas.
* **Almacenamiento:** Volúmenes SSD `gp3` con cifrado KMS en reposo (`storage_encrypted = true`).
* **Acceso Público:** Desactivado de forma obligatoria (`publicly_accessible = false`).
* **Respaldo y Retención:** Backups automáticos continuos durante 7 días y actualización automática de versiones menores.

---

### 4.4. Módulo `modules/secrets` (Gestión de Credenciales)
* **Secretos Aprovisionados:**
  1. `${project_name}/${environment}/db-connection-string`:
     - Construye la cadena canónica de conexión a SQL Server:  
       `Server=${host},${port};Database=StudentRegistrationDb;User Id=${user};Password=${pass};TrustServerCertificate=True;MultipleActiveResultSets=true;`
  2. `${project_name}/${environment}/jwt-signing-key`:
     - Almacena la llave criptográfica HMAC simétrica de alta entropía.
* **Ventana de Recuperación Inmediata:** `recovery_window_in_days = 0` para permitir recreación ágil en entornos de prueba.

---

### 4.5. Módulo `modules/ecs-fargate` (Cómputo Serverless y Balanceo)
* **Clúster ECS:** Configurado con **Container Insights** habilitado para observabilidad granular de CPU y memoria.
* **Application Load Balancer (ALB):**
  - Desplegado en las subredes públicas.
  - Listener HTTP en puerto 80 que reenvía al Target Group.
* **Target Group y Chequeo de Salud (`/healthz`):**
  - Protocolo HTTP, puerto 8080, tipo `ip`.
  - Endpoint de salud: `/healthz` con código esperado `200`.
  - Intervalo de 30 segundos, timeout de 5 segundos, umbral de salud de 2 respuestas exitosas consecutivas.
* **Task Definition (Fargate):**
  - Configuración liviana de recursos: 256 CPU units (0.25 vCPU) y 512 MB de RAM.
  - Inyección de secretos de Secrets Manager mediante la convención de doble guión bajo (`__`) de .NET:
    ```json
    "secrets": [
      { "name": "ConnectionStrings__DefaultConnection", "valueFrom": "<arn-db-secret>" },
      { "name": "Jwt__SecretKey", "valueFrom": "<arn-jwt-secret>" }
    ]
    ```
  - Logging centralizado hacia CloudWatch (`/ecs/student-registration-prod-api`).
* **Servicio ECS Fargate:**
  - 2 réplicas distribuidas en las subredes privadas para garantizar alta disponibilidad ante caída de zona.
  - Sin IP pública asignada (`assign_public_ip = false`).

---

### 4.6. Módulo `modules/frontend-s3-cf` (Alojamiento y CDN para Angular 21)
* **Bucket S3 Privado:**
  - Bloqueo de acceso público total (`BlockPublicAcls`, `BlockPublicPolicy`, `IgnorePublicAcls`, `RestrictPublicBuckets` en `true`).
  - Cifrado en reposo habilitado (`AES256`).
* **Origin Access Control (OAC):**
  - Firma criptográfica SigV4 obligatoria entre CloudFront y S3.
  - Política de bucket que solo autoriza `s3:GetObject` si la solicitud proviene del ARN de la distribución de CloudFront.
* **Enrutamiento SPA (Single Page Application):**
  - Reglas personalizadas de error HTTP 403 y 404 redirigiendo a `/index.html` con status `200`.
  - Permite que las rutas virtuales de Angular (`/catalog`, `/login`, `/admin`, `/my-enrollments`) y las guardas (`studentGuard`, `adminGuard`) funcionen de manera nativa sin recargas fallidas.
* **Políticas de Caché:** Política administrada `CachingOptimized` con compresión gzip/brotli automática y redirección obligatoria a HTTPS.

---

## 5. Integración con el Código Desarrollado

| Componente del Proyecto | Recurso AWS en Terraform | Mecanismo de Integración |
| :--- | :--- | :--- |
| **API .NET 8 (Backend)** | Tareas de AWS ECS Fargate | Ejecución del contenedor basado en el [`src/backend/Dockerfile`](file:///c:/Users/alvar/Documents/Prueba%20inter/src/backend/Dockerfile) en puerto 8080 con usuario no-root. |
| **Health Check API** | ALB Target Group (`/healthz`) | El endpoint registrado en [`src/backend/Adapters/Api/Program.cs`](file:///c:/Users/alvar/Documents/Prueba%20inter/src/backend/Adapters/Api/Program.cs#L41-L47) valida la disponibilidad del servicio. |
| **Persistencia EF Core** | Amazon RDS SQL Server | La conexión se abre vía T-SQL en puerto 1433 en subredes aisladas; [`DbInitializer.cs`](file:///c:/Users/alvar/Documents/Prueba%20inter/src/backend/Adapters/Infrastructure/Persistence/DbInitializer.cs) ejecuta las semillas automáticamente en arranque. |
| **Configuración `appsettings.json`** | AWS Secrets Manager | Fargate inyecta `ConnectionStrings__DefaultConnection` y `Jwt__SecretKey` sobrescribiendo las variables locales sin tocar código. |
| **Angular 21 SPA (Frontend)** | Amazon S3 + CloudFront | El contenido generado en `src/frontend/dist/frontend` (~327 KB) se sube a S3 y se sirve acelerado globalmente por CloudFront. |

---

## 6. Guía de Ejecución y Despliegue de Terraform

### Prerrequisitos
1. **AWS CLI** instalado y configurado con credenciales de una cuenta AWS con permisos de infraestructura (`aws configure`).
2. **Terraform CLI** versión $\ge 1.5.0$.
3. **Imagen Docker** de la API compilada y subida a Amazon ECR.

### Pasos de Despliegue

1. **Crear archivo de variables locales:**
   Copiar la plantilla de ejemplo y completar los valores sensibles:
   ```bash
   cp terraform/terraform.tfvars.example terraform/terraform.tfvars
   ```

2. **Inicializar el entorno de Terraform:**
   Descarga el provider de AWS y prepara los módulos:
   ```bash
   cd terraform
   terraform init
   ```

3. **Validar y formatear la sintaxis:**
   ```bash
   terraform fmt -check
   terraform validate
   ```

4. **Planificar la infraestructura (Dry-run):**
   Genera el plan de ejecución y verifica los recursos que se van a crear (VPC, subredes, RDS, ECS, S3, CloudFront):
   ```bash
   terraform plan -out=tfplan
   ```

5. **Aplicar los cambios (Aprovisionamiento):**
   ```bash
   terraform apply tfplan
   ```

6. **Consultar URLs y salidas tras el despliegue:**
   ```bash
   terraform output
   ```
   *Retornará:*
   - `frontend_url`: `https://d123456789.cloudfront.net` (Acceso web a la plataforma).
   - `api_base_url`: `http://student-registration-prod-alb-...us-east-1.elb.amazonaws.com` (Punto de acceso API).
   - `s3_bucket_name`: Bucket para el pipeline de sincronización de Angular.
   - `cloudfront_distribution_id`: Para invalidar la caché (`aws cloudfront create-invalidation`).

7. **Destrucción de Recursos (si se requiere desmantelar):**
   ```bash
   terraform destroy -auto-approve
   ```

---

## 7. Matriz de Decisiones Técnicas y Beneficios

| Decisión de Infraestructura | Alternativa Descartada | Justificación Técnica | Beneficio Directo |
| :--- | :--- | :--- | :--- |
| **ECS Fargate (Serverless)** | Máquinas virtuales Amazon EC2 | Eliminar la gestión de SO, parches de seguridad y dimensionamiento de clústeres. | Menor sobrecarga operativa y escalado nativo. |
| **Subredes Aisladas para RDS** | RDS en subredes públicas con IP | Protección contra ataques directos de fuerza bruta e intrusiones de red externa. | Máxima seguridad perimetral para datos académicos. |
| **CloudFront con OAC** | Bucket S3 público como Static Website | Restringir el tráfico para que solo fluya a través del CDN con cifrado HTTPS y protección DDoS. | Cumplimiento de seguridad y aceleración global. |
| **Manejo de Errores SPA (403/404 $\to$ index.html)** | Página de error 404 por defecto de S3 | Las aplicaciones SPA gestionan las rutas internamente; S3 daría error al recargar `/catalog`. | Experiencia de usuario fluida y persistencia de rutas en Angular. |
| **Inyección de Secretos vía Fargate** | Variables de entorno en texto plano | Evitar almacenar contraseñas en repositorios Git o en la consola de AWS. | Cumplimiento normativo y seguridad de credenciales. |
