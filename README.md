# Entrega Técnica Integral: Líder Técnico & Desarrollo Web
**Empresa:** INTER RAPIDÍSIMO S.A.  
**Candidato:** Líder Técnico de Desarrollo y Arquitectura Cloud  
**Tecnologías:** ASP.NET Core (.NET 8), Arquitectura Hexagonal, Mediador Nativo, Specification Pattern, Angular 21 (Standalone + Signals), Microsoft SQL Server, AWS Cloud.

---

## 📌 Resumen Ejecutivo de Entregables

La prueba técnica solicitada por Inter Rapidísimo se compone de dos grandes bloques, ambos desarrollados al 100%:

### 1. Bloque Teórico y Arquitectura de Líder Técnico
* **Ubicación:** [docs/LIDER_TECNICO_SOLUCION.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/LIDER_TECNICO_SOLUCION.md)
* **Contenido:**
  * **10 Preguntas Conceptuales Clave:** Respuestas exhaustivas a nivel de Líder Técnico sobre optimización de SQL Server, índices clustered vs non-clustered, arquitectura elástica en AWS, EC2 vs Lambda, Secrets Manager, CI/CD y despliegues con rollback, ASP.NET Core vs MVC, seguridad en APIs REST y ADO.NET vs EF Core.
  * **Caso de Estudio ZYX (Logística):** Diagnóstico de causas raíz y diseño de arquitectura cloud/híbrida moderna con diagramas Mermaid, desacoplamiento con colas (SQS/EventBridge) para evitar caídas por proveedores de geoposición/facturación, patrón CQRS con réplica de lectura para aislar reportes de la BD transaccional, y modernización de UI ergonómica en Angular.

---

### 2. Bloque Práctico: Aplicación Web de Registro de Estudiantes
Sistema completo de registro de estudiantes y selección de materias cumpliendo rigurosamente todas las restricciones institucionales:
* **CRUD de estudiantes en línea** con autenticación JWT y roles.
* **10 materias disponibles** (3 créditos cada una) y **5 profesores** (2 materias por profesor).
* **Selección de hasta 3 materias (9 créditos)** con soporte de confirmación parcial.
* **Regla innegociable de negocio:** Prohibición estricta de cursar materias con el mismo docente (validado en Dominio con *Specification Pattern* y en Frontend con *Signals reactivos* en tiempo real).
* **Privacidad de datos personales:** Directorio público y vista de compañeros de clase exponiendo única y exclusivamente el nombre completo.

### 3. Documentación Oficial y Manuales en PDF
Para facilitar la lectura y evaluación ejecutiva, se consolidaron tres expedientes completos en formato PDF con diseño corporativo y diagramas vectorizados:
* **[Manual de Usuario y Guía Operativa (PDF)](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/MANUAL_DE_USUARIO_REGISTRO_ACADEMICO.pdf):** Manual visual ilustrado de 9 páginas con las 6 capturas reales de la aplicación en producción en CloudFront (Login, Registro, Catálogo reactivo 9/9 CR con bloqueo de profesor, Mis Materias con retiro y compañeros, Directorio con privacidad y Panel Administrativo).
* **[Expediente de Arquitectura y Solución Técnica Integral (PDF)](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/EXPEDIENTE_ARQUITECTURA_LIDER_TECNICO.pdf):** Dossier de 43 páginas que consolida las 10 respuestas conceptuales clave de nivel Líder Técnico, modernización logística ZYX, arquitectura hexagonal .NET 8, frontend reactivo Angular 21 Signals, esquema relacional SQL Server e infraestructura cloud en AWS con Terraform.
* **[Expediente Técnico de Especificaciones SDD (PDF)](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/EXPEDIENTE_TECNICO_001_REGISTRO_ESTUDIANTES.pdf):** Consolidación secuencial de los 8 documentos de especificación del MVP (Requisitos EARS, OpenAPI 3.1, SQL Schema, Arquitectura, AWS Cloud, Plan Técnico, Backlog de 28 tareas y Auditoría QA con veredicto PASSED).

---

## 🏗️ Artefactos de la Metodología SDD (Spec-Driven Development)

Toda la solución fue construida siguiendo un pipeline formal multi-agente:
1. **Constitución Técnica:** [docs/constitution.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/constitution.md)
2. **Especificación Funcional EARS:** [01-spec.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/01-spec.md)
3. **Contrato Canónico OpenAPI 3.1:** [02-openapi.yaml](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/02-openapi.yaml)
4. **Modelo de Base de Datos y ERD:** [03-schema-spec.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/03-schema-spec.md)
5. **Diseño de Arquitectura Hexagonal:** [04-architecture-spec.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/04-architecture-spec.md)
6. **Topología de Nube en AWS:** [05-cloud-architecture.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/05-cloud-architecture.md)
7. **Plan Técnico y Matriz de Tests:** [06-plan.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/06-plan.md)
8. **Backlog de Tareas:** [07-tasks.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/07-tasks.md)
9. **Reporte de Auditoría QA (Veredicto PASSED):** [08-spec-compliance-report.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/08-spec-compliance-report.md)

---

## 💻 Guía de Despliegue en Entorno Local

### 1. Requisitos Previos (Local)
* **.NET 8 SDK** (v8.0.x o superior) instalado (`dotnet --version`).
* **Node.js** (v20.x o superior LTS) y **npm** (`node -v`, `npm -v`).
* **SQL Server** (SQL Server Express, LocalDB o contenedor Docker de SQL Server).
* **Docker Desktop** (opcional, para ejecución y pruebas en contenedores).

### 2. Ejecución de Pruebas Unitarias
El proyecto cuenta con una suite integral de 18 pruebas unitarias (Dominio y Casos de Uso de Aplicación con 100% de éxito):
```bash
dotnet test src/backend/StudentRegistration.slnx
```

### 3. Puesta en Marcha del Backend (.NET 8)
1. Restaurar dependencias del proyecto:
   ```bash
   dotnet restore src/backend/StudentRegistration.slnx
   ```
2. Ejecutar la API REST:
   ```bash
   dotnet run --project src/backend/Adapters/Api/Api.csproj
   ```
3. La API iniciará con Kestrel. Puedes acceder a la documentación interactiva OpenAPI/Swagger en:
   * **Swagger UI:** `https://localhost:7210/swagger` (o `http://localhost:5000/swagger`)
   * **Health Check:** `https://localhost:7210/healthz`

### 4. Puesta en Marcha del Frontend (Angular 21)
1. Ingresar al directorio del frontend e instalar dependencias:
   ```bash
   cd src/frontend
   npm install
   ```
2. Iniciar el servidor de desarrollo local:
   ```bash
   npm start
   ```
3. Abre tu navegador en **`http://localhost:4200`**.

---

## ☁️ Guía de Despliegue en la Nube (AWS con Terraform)

La infraestructura como código (IaC) modular permite aprovisionar y destruir todo el ecosistema en Amazon Web Services de forma 100% automatizada.

### 1. Requisitos Previos (Nube)
* **Cuenta de AWS** activa (con credenciales de administrador o rol adecuado).
* **AWS CLI v2** instalado y autenticado (`aws configure`).
* **Terraform** (>= v1.5.0) instalado (`terraform -version`).
* **Docker Desktop** activo (para compilar y subir la imagen del contenedor a Amazon ECR).

### 2. Configuración de Parámetros de Terraform
1. Ingresar a la carpeta de infraestructura:
   ```bash
   cd terraform
   ```
2. Crear un archivo de variables locales `terraform.tfvars` (el cual está protegido en `.gitignore` para no subir secretos a Git):
   ```hcl
   aws_region      = "us-east-1"
   project_name    = "student-registration"
   environment     = "dev"
   db_password     = "TuContrasenaSeguraSQL2026!"
   jwt_secret_key  = "ClaveSuperSecretaYRobustaParaFirmaDeTokensJWT2026!"
   desired_count   = 1
   ```

### 3. Despliegue Automatizado en la Nube
1. **Inicializar Terraform:**
   ```bash
   terraform init
   ```

2. **Crear el Repositorio ECR y subir la Imagen Docker:**
   ```powershell
   # Aprovisionar el registro privado en AWS ECR
   terraform apply -target="module.ecs_fargate.aws_ecr_repository.api" -auto-approve

   # Obtener login de Docker en ECR
   $pass = (aws ecr get-login-password --region us-east-1).Trim()
   docker login --username AWS --password $pass <ID_DE_CUENTA>.dkr.ecr.us-east-1.amazonaws.com

   # Compilar y subir la imagen Docker
   docker build -t <ID_DE_CUENTA>.dkr.ecr.us-east-1.amazonaws.com/student-registration-dev-api:latest -f ../src/backend/Dockerfile ../src/backend
   docker push <ID_DE_CUENTA>.dkr.ecr.us-east-1.amazonaws.com/student-registration-dev-api:latest
   ```

3. **Desplegar Toda la Infraestructura con un Solo Comando:**
   ```bash
   terraform apply -auto-approve
   ```
   *Este comando aprovisiona de forma desatendida: VPC con 6 subredes en 2 AZs, NAT Gateway, Security Groups, Amazon RDS SQL Server (`db.t3.micro`), Clúster ECS Fargate, Application Load Balancer con `/healthz`, Secretos en AWS Secrets Manager, Bucket S3 privado cifrado y Distribución Global Amazon CloudFront con Reverse Proxy en `/api/*`.*

4. **Publicar el Frontend Angular en Amazon S3 y CloudFront:**
   ```powershell
   cd ../src/frontend
   npm run build
   aws s3 sync dist/frontend/browser/ s3://student-registration-dev-<ID_DE_CUENTA>-frontend --delete
   aws cloudfront create-invalidation --distribution-id <ID_DISTRIBUCION_CLOUDFRONT> --paths "/*"
   ```

### 4. Limpieza y Desmantelamiento (Costo Cero Continuo)
Cuando finalices las pruebas y evaluación de la solución en la nube, puedes desmantelar todos los recursos ejecutando:
```bash
cd terraform
terraform destroy -auto-approve
```
*Garantiza la eliminación íntegra de los 47 recursos aprovisionados sin dejar instancias huérfanas ni costos recurrentes en tu cuenta de AWS.*

---

## 👤 Credenciales de Demostración Iniciales

* **Usuario Administrador:** `admin@universidad.edu.co` / `Admin123!`
* **Estudiantes:** Pueden autorregistrarse directamente desde la pantalla de bienvenida `/register`.
