# Plan Técnico de Implementación (`plan.md`)

## 1. Resumen
Este plan establece el diseño técnico, la arquitectura modular, las decisiones de diseño y la estrategia de verificación para construir el MVP del Sistema de Registro de Estudiantes e Inscripción Académica, basándose en la especificación [spec.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/spec.md) y los principios innegociables de [constitution.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/constitution.md).

---

## 2. Estructura de Módulos (Arquitectura Hexagonal)

### 2.1 Capa de Dominio (`Core/Domain/`)
* **Responsabilidad:** Encapsular las entidades del modelo, sus invariantes y las especificaciones puras del negocio sin dependencias externas.
* **Componentes:**
  * `Entities`: `Student`, `Course`, `Professor`, `Enrollment`, `User`.
  * `Specifications`:
    * `MaxCoursesSpecification`: Garantiza que el alumno no supere 3 asignaturas (9 créditos). (Cubre: RF-4)
    * `UniqueProfessorSpecification`: Asegura que el profesor de la nueva asignatura no coincida con el profesor de materias ya inscritas. (Cubre: RF-4)
    * `NotAlreadyEnrolledSpecification`: Impide inscribir una asignatura repetida. (Cubre: RF-4)
  * `Exceptions`: `DomainValidationException`, `ConflictException`, `NotFoundException`, `UnauthorizedException`. (Cubre: RF-1, RF-2, RF-4, RF-8)

### 2.2 Capa de Aplicación (`Core/Application/`)
* **Responsabilidad:** Orquestar los casos de uso, manejar transacciones y definir puertos de entrada/salida.
* **Componentes:**
  * `Dispatcher Nativo`: `IRequest<T>`, `IRequestHandler<T, R>`, `IDispatcher` implementado con `IServiceProvider` de .NET sin dependencias NuGet externas. (Cubre todos los RF)
  * `Features/Auth`: `RegisterStudentCommand`, `LoginCommand` y sus handlers. (Cubre: RF-1, RF-2)
  * `Features/Enrollments`: `EnrollCourseCommand`, `DropCourseCommand`, `GetMyEnrollmentsQuery`, `GetCourseClassmatesQuery` y sus handlers. (Cubre: RF-4, RF-5, RF-7)
  * `Features/Catalog`: `GetCoursesQuery`, `GetPublicStudentsQuery` y sus handlers. (Cubre: RF-3, RF-6)
  * `Features/Admin`: Comandos y consultas para gestión de docentes y asignaturas. (Cubre: RF-8)
  * `Ports`: Interfaces `IStudentRepository`, `ICourseRepository`, `IProfessorRepository`, `IEnrollmentRepository`, `IUserRepository`, `IPasswordHasher`, `ITokenService`, `IUnitOfWork`.

### 2.3 Capa de Infraestructura (`Adapters/Infrastructure/`)
* **Responsabilidad:** Implementar los puertos secundarios hacia base de datos SQL Server y servicios de seguridad.
* **Componentes:**
  * `Persistence`: `ApplicationDbContext`, configuraciones Fluent API (`EntityTypeConfiguration`), soft-delete global e implementaciones de repositorios. (Cubre: RF-1, RF-3, RF-4, RF-5, RF-6, RF-7, RF-8)
  * `Security`: `Pbkdf2PasswordHasher` (derivación criptográfica de contraseñas) y `JwtTokenService` (generación y validación de tokens). (Cubre: RF-1, RF-2)

### 2.4 Capa de Transporte / API (`Adapters/Api/`)
* **Responsabilidad:** Adaptador primario HTTP que expone los endpoints de `openapi.yaml`.
* **Componentes:**
  * Controladores delgados: `AuthController`, `StudentsController`, `CoursesController`, `AdminController`.
  * `Middleware`: Interceptor de errores que transforma excepciones a RFC 7807 (`ProblemDetails`). (Cubre: RF-1 a RF-8)

---

## 3. Decisiones Técnicas

1. **Mediador Nativo sin dependencias de terceros:**
   * *Decisión:* Implementar `IDispatcher` interno en C# mediante el contenedor DI de .NET en lugar del paquete MediatR.
   * *Justificación:* Cumplimiento estricto del Principio 1 de la Constitución (simplicidad del stack y prohibición expresa de MediatR).
   * *Alternativa descartada:* MediatR 12.x; descartada por violación constitucional.
   * *RF Afectados:* Todos.

2. **Specification Pattern en Dominio:**
   * *Decisión:* Encapsular las reglas de créditos y profesor no repetido en clases `ISpecification<T>` reutilizables y comprobables de forma aislada.
   * *Justificación:* Principio 3 (separación de lógica e interfaz) y principio 4 (tests unitarios puros sin dependencias de base de datos).
   * *Alternativa descartada:* Validación procedimental dentro de controllers o dentro de queries SQL; descartada porque rompe el aislamiento del dominio.
   * *RF Afectados:* RF-4, RF-5.

3. **Autenticación JWT con PBKDF2:**
   * *Decisión:* Emplear tokens JWT simétricos (HMAC-SHA256) con claims `role` y hash de contraseñas con `Rfc2898DeriveBytes` (PBKDF2 con sal aleatoria de 16 bytes e iteraciones estándar).
   * *Justificación:* Seguridad y cumplimiento de RF-2 sin añadir paquetes externos pesados.
   * *Alternativa descartada:* Identity de ASP.NET completo; descartado por sobrecarga de dependencias innecesarias para un MVP de 2 roles.
   * *RF Afectados:* RF-1, RF-2.

4. **Protección de Privacidad por Diseño (DTOs Dedicados):**
   * *Decisión:* Separar tajantemente los DTOs: `PublicStudentDto` (solo `FullName`) y `ClassmateDto` (solo `FullName`) frente a `StudentProfileDto` (privado).
   * *Justificación:* Garantiza a nivel de tipo de retorno que jamás se filtren emails o identificaciones en endpoints compartidos (RF-6, RF-7).
   * *Alternativa descartada:* Reutilizar un único DTO de estudiante con campos opcionales/nulos; descartada por riesgo de fuga de datos.
   * *RF Afectados:* RF-6, RF-7.

---

## 4. Estrategia de Tests
* **Framework:** xUnit con FluentAssertions.
* **Dominio (Unit Tests):** Cobertura del 100% en las especificaciones (`MaxCoursesSpecification`, `UniqueProfessorSpecification`, `NotAlreadyEnrolledSpecification`).
* **Aplicación (Unit Tests):** Pruebas de casos de uso simulando repositorios mediante mocks/dobles para verificar la orquestación y el manejo de excepciones.
* **Meta de Cobertura:** Cobertura combinada en `Domain` y `Application` superior al **85%** medible mediante `dotnet test /p:CollectCoverage=true`.

---

## 5. Matriz de Cobertura RF → Plan → Test

| RF | Módulo Responsable | Nombre Previsto del Test | Tipo de Prueba |
| :--- | :--- | :--- | :---: |
| **RF-1** | `Application/Auth` | `RegisterStudent_WithValidData_ReturnsSuccessAndToken` | Unitaria |
| **RF-1** | `Application/Auth` | `RegisterStudent_WithExistingEmailOrId_ThrowsConflictException` | Unitaria |
| **RF-1** | `Application/Auth` | `RegisterStudent_WithShortPassword_ThrowsValidationException` | Unitaria |
| **RF-2** | `Application/Auth` | `Login_WithValidCredentials_ReturnsJwtToken` | Unitaria |
| **RF-2** | `Application/Auth` | `Login_WithInvalidCredentials_ThrowsUnauthorizedException` | Unitaria |
| **RF-3** | `Application/Catalog` | `GetCourses_ReturnsCatalogWithAssignedProfessors` | Unitaria |
| **RF-4** | `Domain/Specifications` | `MaxCoursesSpecification_WhenUnderThreeCourses_IsSatisfied` | Unitaria |
| **RF-4** | `Domain/Specifications` | `MaxCoursesSpecification_WhenThreeCoursesAlreadyEnrolled_Fails` | Unitaria |
| **RF-4** | `Domain/Specifications` | `UniqueProfessorSpecification_WhenProfessorNotEnrolled_IsSatisfied` | Unitaria |
| **RF-4** | `Domain/Specifications` | `UniqueProfessorSpecification_WhenProfessorAlreadyEnrolled_Fails` | Unitaria |
| **RF-4** | `Domain/Specifications` | `NotAlreadyEnrolledSpecification_WhenCourseAlreadyEnrolled_Fails` | Unitaria |
| **RF-4** | `Application/Enrollments` | `EnrollCourse_WithValidConditions_CreatesEnrollmentAndUpdatesCredits` | Unitaria |
| **RF-5** | `Application/Enrollments` | `DropCourse_WhenEnrolled_RemovesEnrollmentAndReleasesCredits` | Unitaria |
| **RF-6** | `Application/Catalog` | `GetPublicStudents_ReturnsOnlyFullNamesAndHidesSensitiveData` | Unitaria |
| **RF-7** | `Application/Enrollments` | `GetCourseClassmates_ReturnsOnlyClassmateFullNames` | Unitaria |
| **RF-8** | `Application/Admin` | `DeleteProfessor_WhenHasActiveEnrolledStudents_ThrowsConflictException` | Unitaria |
| **RF-8** | `Application/Admin` | `DeleteCourse_WhenHasActiveEnrollments_ThrowsConflictException` | Unitaria |

---

## 6. Riesgos y Bloqueos
* **Sin bloqueos identificados:** Todos los requerimientos, contratos y restricciones arquitectónicas están acordados y cerrados.

---

## 7. Declaración de Verificación
* **Cobertura total:** Los 8 requisitos funcionales (`RF-1` al `RF-8`) están cubiertos en la matriz.
* **Tests nominados:** Cada fila cuenta con un nombre de test concreto y descriptivo.
* **Cobertura medible:** Se utilizará xUnit y el colector de cobertura de .NET con umbral mínimo del 85%.
* **Conformidad:** Cumple al 100% la constitución (Mediador nativo sin MediatR, Hexagonal, idioma y pruebas).
* **Alcance:** Excluye cupos y cruces de horario según lo acordado.
