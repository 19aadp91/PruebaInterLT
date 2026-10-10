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

---

## 🏗️ Artefactos de la Metodología SDD (Spec-Driven Development)

Toda la solución fue construida siguiendo un pipeline formal multi-agente:
1. **Constitución Técnica:** [docs/constitution.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/constitution.md)
2. **Especificación Funcional EARS:** [spec.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/spec.md)
3. **Contrato Canónico OpenAPI 3.1:** [openapi.yaml](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/openapi.yaml)
4. **Modelo de Base de Datos y ERD:** [schema-spec.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/schema-spec.md)
5. **Diseño de Arquitectura Hexagonal:** [architecture-spec.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/architecture-spec.md)
6. **Topología de Nube en AWS:** [cloud-architecture.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/cloud-architecture.md)
7. **Plan Técnico y Matriz de Tests:** [plan.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/plan.md)
8. **Backlog de Tareas:** [tasks.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/tasks.md)
9. **Reporte de Auditoría QA (Veredicto PASSED):** [spec-compliance-report.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/spec-compliance-report.md)

---

## 🚀 Guía de Ejecución Rápida

### 1. Base de Datos (SQL Server)
* El script DDL con tablas, constraints, índices de rendimiento y datos semilla (los 5 profesores y 10 materias precargadas) se encuentra en:
  `database/scripts/01_init_schema_and_seeds.sql`
* *Nota:* Para facilitar la evaluación inmediata, el backend incluye inicialización automática de semillas en memoria o SQL Server.

### 2. Backend (.NET 8)
Para ejecutar las pruebas unitarias automatizadas (15/15 pruebas superadas):
```bash
dotnet test src/backend/StudentRegistration.slnx
```

Para levantar la API HTTP:
```bash
dotnet run --project src/backend/Adapters/Api/Api.csproj
```
La API quedará escuchando en `http://localhost:5000` (o el puerto configurado por Kestrel).

### 3. Frontend (Angular Standalone con Signals)
Para compilar y levantar el servidor de desarrollo:
```bash
cd src/frontend
npm start
```
Abre tu navegador en `http://localhost:4200`.

---

## 👤 Credenciales de Demostración Iniciales

* **Usuario Administrador:** `admin@universidad.edu.co` / `Admin123!`
* **Estudiantes:** Pueden autorregistrarse directamente desde la pantalla de bienvenida `/register`.
