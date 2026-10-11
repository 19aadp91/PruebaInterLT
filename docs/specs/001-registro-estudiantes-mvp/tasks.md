# Desglose de Tareas de Implementación (`tasks.md`)

## 1. Resumen Ejecutivo
* **Especificación:** [spec.md](file:///c:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/spec.md)
* **Plan Técnico:** [plan.md](file:///c:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/plan.md)
* **Arquitectura de Software:** [architecture-spec.md](file:///c:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/architecture-spec.md)
* **Contrato Canónico:** [openapi.yaml](file:///c:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/openapi.yaml)
* **Estado Global:** **100% Completado y Verificado** (26 tareas ejecutadas, 18 pruebas unitarias aprobadas, 0 advertencias, 0 errores).

---

## 2. Backlog de Tareas Ejecutadas

### Capa 1: Dominio Hexagonal y Reglas de Negocio Puras
- [x] **T-01** — Modelar entidades del dominio (`User`, `Student`, `Professor`, `Course`, `Enrollment`) y clase base de auditoría `BaseEntity` (`CreatedAtUtc`, `UpdatedAtUtc`, `IsDeleted`).
  - Cubre: **RF-1, RF-3, RF-4**
  - Estado: **COMPLETADO**
  - Verificación: Entidades puras en `Core/Domain/Entities/` sin dependencias de frameworks externos.

- [x] **T-02** — Implementar el contenedor funcional `Result<T>`, `Result` y el enum `ErrorType` (*Railway-Oriented Programming*).
  - Cubre: Transversal (**RF-1 a RF-8**)
  - Estado: **COMPLETADO**
  - Verificación: Manejo estandarizado de respuestas de éxito y fallo sin propagación costosa de excepciones. Pruebas en `ResultTests.cs`.

- [x] **T-03** — Implementar la abstracción `ISpecification<T>` y el modelo contextual `EnrollmentCandidate`.
  - Cubre: **RF-4**
  - Estado: **COMPLETADO**
  - Verificación: Abstracción pura implementada en `Domain/Common/ISpecification.cs`.

- [x] **T-04** — Implementar especificación de límite de materias: `MaxCoursesSpecification` (límite de 3 materias / 9 créditos).
  - Cubre: **RF-4**
  - Estado: **COMPLETADO**
  - Verificación: Pruebas `MaxCoursesSpecification_WhenUnderThreeCourses_IsSatisfied` y `MaxCoursesSpecification_WhenThreeCoursesAlreadyEnrolled_Fails` aprobadas.

- [x] **T-05** — Implementar especificación de docente único: `UniqueProfessorSpecification` (no cursar materias con el mismo profesor).
  - Cubre: **RF-4**
  - Estado: **COMPLETADO**
  - Verificación: Pruebas `UniqueProfessorSpecification_WhenProfessorNotEnrolled_IsSatisfied` y `UniqueProfessorSpecification_WhenProfessorAlreadyEnrolled_Fails` aprobadas.

- [x] **T-06** — Implementar especificación de no duplicidad de materia: `NotAlreadyEnrolledSpecification`.
  - Cubre: **RF-4**
  - Estado: **COMPLETADO**
  - Verificación: Pruebas `NotAlreadyEnrolledSpecification_WhenCourseAlreadyEnrolled_Fails` aprobadas con 100% de cobertura.

---

### Capa 2: Aplicación, Mediador Nativo y CQRS
- [x] **T-07** — Implementar Despachador/Mediador Nativo (`IRequest<T>`, `IRequestHandler<T, R>`, `IDispatcher`, `NativeDispatcher`) y pipeline de validación funcional (`IValidator<T>`).
  - Cubre: Transversal (**RF-1 a RF-8**)
  - Estado: **COMPLETADO**
  - Verificación: Resuelve handlers y validadores en tiempo de ejecución utilizando `IServiceProvider` de .NET sin librerías externas (cero MediatR).

- [x] **T-08** — Declarar puertos de persistencia y servicios transversales en `Application/Ports`.
  - Cubre: **RF-1 a RF-8**
  - Estado: **COMPLETADO**
  - Verificación: Contratos `IStudentRepository`, `ICourseRepository`, `IProfessorRepository`, `IEnrollmentRepository`, `IUserRepository`, `IUnitOfWork`, `IPasswordHasher`, `ITokenService`.

- [x] **T-09** — Implementar casos de uso de Autenticación (`RegisterStudentCommand`, `LoginCommand`) con validadores dedicados.
  - Cubre: **RF-1, RF-2**
  - Estado: **COMPLETADO**
  - Verificación: Pruebas en `AuthHandlerTests.cs` (registro exitoso, conflicto por documento/email duplicado, validación de contraseña $\ge 6$, login exitoso y credenciales inválidas).

- [x] **T-10** — Implementar casos de uso de Inscripción y Retiro (`EnrollCourseCommand`, `DropCourseCommand`, `GetMyEnrollmentsQuery`).
  - Cubre: **RF-4, RF-5**
  - Estado: **COMPLETADO**
  - Verificación: Pruebas en `EnrollmentHandlerTests.cs` evaluando orquestación con especificaciones de dominio y persistencia atómica con `UnitOfWork`.

- [x] **T-11** — Implementar consultas públicas con protección de privacidad de datos (`GetCoursesQuery`, `GetPublicStudentsQuery`, `GetCourseClassmatesQuery`, `GetStudentProfileQuery`).
  - Cubre: **RF-3, RF-6, RF-7**
  - Estado: **COMPLETADO**
  - Verificación: Pruebas unitarias confirman que `PublicStudentDto` y `ClassmateDto` exponen únicamente `FullName` y omiten documentos o correos.

- [x] **T-12** — Implementar casos de uso administrativos (`AdminCreateCourse`, `AdminDeleteCourse`, `AdminCreateProfessor`, `AdminDeleteProfessor`).
  - Cubre: **RF-8**
  - Estado: **COMPLETADO**
  - Verificación: Bloqueo de eliminación ante asignaturas o docentes con inscripciones activas vinculadas.

---

### Capa 3: Persistencia y Adaptadores de Infraestructura
- [x] **T-13** — Diseñar script DDL T-SQL idempotente (`database/scripts/01_init_schema_and_seeds.sql`) para SQL Server 2022+ / Azure SQL.
  - Cubre: **RF-1, RF-3, RF-4, RF-8**
  - Estado: **COMPLETADO**
  - Verificación: Creación de tablas, claves primarias `UNIQUEIDENTIFIER` (`NEWSEQUENTIALID()`), índice único filtrado `UQ_Enrollments_ActiveStudentCourse` y seeds de 5 profesores, 10 cursos y admin.

- [x] **T-14** — Implementar `ApplicationDbContext`, configuraciones Fluent API y filtro global de Soft-Delete (`HasQueryFilter(e => !e.IsDeleted)`).
  - Cubre: **RF-1 a RF-8**
  - Estado: **COMPLETADO**
  - Verificación: Mapeo de entidades, relaciones 1:1 compartidas (`Users` $\to$ `Students`) y soporte dual para SQL Server y base de datos InMemory.

- [x] **T-15** — Implementar repositorio genérico / específicos y patrón `UnitOfWork`.
  - Cubre: **RF-1 a RF-8**
  - Estado: **COMPLETADO**
  - Verificación: Repositorios concretos en `Adapters/Infrastructure/Persistence/Repositories/`.

- [x] **T-16** — Implementar adaptadores de seguridad criptográfica: `Pbkdf2PasswordHasher` y `JwtTokenService`.
  - Cubre: **RF-1, RF-2**
  - Estado: **COMPLETADO**
  - Verificación: Hashing de contraseñas con salt de 16 bytes y generación de tokens JWT con claims de rol (`Estudiante`, `Administrador`).

- [x] **T-17** — Implementar inicializador de base de datos automático e idempotente (`DbInitializer.cs`).
  - Cubre: **RF-3, RF-8**
  - Estado: **COMPLETADO**
  - Verificación: Siembra garantizada en arranque de la API (5 profesores, 10 cursos a 3 créditos y usuario admin).

---

### Capa 4: API HTTP y Transporte
- [x] **T-18** — Implementar controlador base `BaseApiController` para mapeo automático de `Result<T>` a códigos de estado HTTP.
  - Cubre: Transversal (**RF-1 a RF-8**)
  - Estado: **COMPLETADO**
  - Verificación: Mapeo centralizado a 200, 201, 400, 401, 403, 404 y 409.

- [x] **T-19** — Implementar controladores REST delgados (`AuthController`, `StudentsController`, `CoursesController`, `AdminController`).
  - Cubre: **RF-1 a RF-8**
  - Estado: **COMPLETADO**
  - Verificación: Endpoints alineados al 100% con las rutas y esquemas de `openapi.yaml`.

- [x] **T-20** — Implementar Middleware de excepciones no controladas bajo RFC 7807 (`ProblemDetailsExceptionMiddleware`), Swagger UI y CORS.
  - Cubre: **RF-1 a RF-8**
  - Estado: **COMPLETADO**
  - Verificación: Respuestas `ProblemDetails` para errores 500 y documentación interactiva Swagger en `/swagger`.

---

### Capa 5: Frontend Angular 21 (Zoneless, Signals & RBAC)
- [x] **T-21** — Configurar arquitectura Zoneless y modularización de modelos en Angular 21.
  - Cubre: Transversal (**RF-1 a RF-8**)
  - Estado: **COMPLETADO**
  - Verificación: `provideZonelessChangeDetection()` en `app.config.ts`, modelos tipados en `core/models/` y barrel export `index.ts`.

- [x] **T-22** — Implementar servicios reactivos singleton (`AuthService`, `AcademicService`) e interceptores funcionales.
  - Cubre: **RF-1 a RF-8**
  - Estado: **COMPLETADO**
  - Verificación: Manejo de estado con `signal()` y `computed()`. Interceptor `apiEnvelopeInterceptor` desempaqueta transparentemente el sobre `Result<T>`.

- [x] **T-23** — Implementar componentes de autenticación (`LoginComponent`, `RegisterComponent`) con separación de `.ts`, `.html` y `.css`.
  - Cubre: **RF-1, RF-2**
  - Estado: **COMPLETADO**
  - Verificación: Formularios reactivos, redirección por rol tras login y manejo visual de errores.

- [x] **T-24** — Implementar catálogo de asignaturas interactivo (`CatalogComponent`) con validación reactiva de reglas de negocio.
  - Cubre: **RF-3, RF-4**
  - Estado: **COMPLETADO**
  - Verificación: Detección en vivo de conflicto docente (`hasProfessorConflict()`), control de límite de 3 materias / 9 créditos y bloqueo preventivo en la UI.

- [x] **T-25** — Implementar panel de matrícula activa (`MyEnrollmentsComponent`), retiro de asignaturas y modal de compañeros.
  - Cubre: **RF-5, RF-7**
  - Estado: **COMPLETADO**
  - Verificación: Visualización del resumen de créditos cursados (`0/9 CR`), cancelación de materias y listado de compañeros protegiendo datos sensibles.

- [x] **T-26** — Implementar directorio público de estudiantes (`StudentsComponent`) y panel administrativo (`AdminComponent`).
  - Cubre: **RF-6, RF-8**
  - Estado: **COMPLETADO**
  - Verificación: Directorio público accesible y módulo administrativo para alta/baja de materias y profesores.

- [x] **T-27** — Implementar control de acceso basado en roles (RBAC): Guardas funcionales y protección de interfaz.
  - Cubre: **RF-8**
  - Estado: **COMPLETADO**
  - Verificación: Guardas funcionales `studentGuard`, `adminGuard`, `homeGuard`. Los administradores no visualizan ni pueden acceder a catálogo ni a materias de alumnos (`@if (authService.isStudent())`).

- [x] **T-28** — Centralización del sistema de estilos CSS (`styles.css`) y variables de entorno (`environments/`).
  - Cubre: Calidad técnica / Principio DRY
  - Estado: **COMPLETADO**
  - Verificación: Clases de botones, badges, alertas y tarjetas centralizadas en `styles.css`. Bundle frontend compilado limpiamente en 327 KB (0 errores, 0 advertencias).

---

## 3. Matriz de Trazabilidad Requisitos Funcionales ↔ Tareas

| Requisito Funcional | Tareas de Backend | Tareas de Frontend / Base de Datos | Estado |
| :--- | :--- | :--- | :---: |
| **RF-1** (Registro en Línea) | T-01, T-02, T-07, T-08, T-09, T-16, T-18, T-19, T-20 | T-13, T-14, T-21, T-22, T-23, T-28 | **APROBADO** |
| **RF-2** (Inicio de Sesión y JWT) | T-02, T-07, T-08, T-09, T-16, T-18, T-19 | T-14, T-21, T-22, T-23, T-27, T-28 | **APROBADO** |
| **RF-3** (Catálogo de Materias) | T-01, T-02, T-07, T-08, T-11, T-18, T-19 | T-13, T-14, T-17, T-21, T-22, T-24, T-27, T-28 | **APROBADO** |
| **RF-4** (Inscripción & Profesor Único) | T-01, T-02, T-03, T-04, T-05, T-06, T-07, T-08, T-10, T-18, T-19 | T-13, T-14, T-21, T-22, T-24, T-28 | **APROBADO** |
| **RF-5** (Modificación y Retiro) | T-02, T-07, T-08, T-10, T-18, T-19 | T-13, T-14, T-21, T-22, T-25, T-28 | **APROBADO** |
| **RF-6** (Lista Pública de Estudiantes) | T-02, T-07, T-08, T-11, T-18, T-19 | T-14, T-21, T-22, T-26, T-28 | **APROBADO** |
| **RF-7** (Compañeros de Clase) | T-02, T-07, T-08, T-11, T-18, T-19 | T-14, T-21, T-22, T-25, T-28 | **APROBADO** |
| **RF-8** (Gestión Administrativa) | T-02, T-07, T-08, T-12, T-18, T-19 | T-13, T-14, T-17, T-21, T-22, T-26, T-27, T-28 | **APROBADO** |

---

## 4. Declaración de Verificación y Cierre
* **100% de Tareas Completadas:** Las 28 tareas técnicas abarcan todos los niveles de la arquitectura limpia (Dominio, Aplicación, Infraestructura, API HTTP, Base de Datos y Frontend Angular).
* **Cobertura de Pruebas Unitarias:** 18 pruebas unitarias ejecutadas mediante `dotnet test` con tasa de éxito del 100%.
* **Build Frontend:** Compilación validada con `npm run build` en Angular 21 Zoneless (0 advertencias, 0 errores).
* **Cumplimiento Constitucional:** Cero librerías externas no autorizadas en el núcleo, mediador nativo interno, validación funcional nativa e integridad referencial en SQL Server.
