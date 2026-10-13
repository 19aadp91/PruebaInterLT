# Especificación de Arquitectura de Software (`04-architecture-spec.md`)

## 1. Identidad y Principios
* **Estilo Arquitectónico:** Arquitectura Hexagonal (Puertos y Adaptadores) fusionada con Clean Architecture y CQRS para Backend; Arquitectura Basada en Componentes Standalone y Reactividad Zoneless con Signals para Frontend.
* **Patrón de Despacho CQRS:** Mediador Nativo Interno (`IDispatcher` / `IRequestHandler<T, R>`), sin dependencias externas como MediatR.
* **Validación Funcional en Capas:** Pipeline nativo de validación con `IValidator<T>` ejecutado antes del handler correspondiente.
* **Control de Flujo Funcional:** Adopción del **Result Pattern** (*Railway-Oriented Programming*) para control de errores y validaciones de negocio sin lanzamiento costoso de excepciones.
* **Patrón de Negocio en Dominio:** Specification Pattern (`ISpecification<T>`) para la evaluación pura y desacoplada de reglas e invariantes de inscripción.
* **Segregación Estricta de Roles (RBAC):** Separación tajante entre perfiles de `Estudiante` y `Administrador` tanto en rutas y controladores backend como en guards funcionales e interfaces frontend.
* **Frontend Zoneless:** Adopción de **Angular 21 Zoneless** (`provideZonelessChangeDetection`), prescindiendo de `zone.js` para máximo rendimiento y predictibilidad.

---

## 2. Backend: Arquitectura Hexagonal (.NET 8)

### 2.1 Descomposición en Capas y Puertos

```text
src/backend/
├── Core/
│   ├── Domain/                             # Núcleo puro de dominio (Cero dependencias externas)
│   │   ├── Common/                         # BaseEntity, ISpecification<T>, Result<T>, ErrorType
│   │   ├── Entities/                       # User, Student, Professor, Course, Enrollment
│   │   └── Specifications/                 # Reglas de negocio puras encapsuladas
│   │       └── EnrollmentSpecifications.cs # MaxCourses, UniqueProfessor, NotAlreadyEnrolled
│   └── Application/                        # Casos de uso y orquestación CQRS
│       ├── Common/
│       │   ├── Dispatcher/                 # MEDIADOR NATIVO (Interfaces e implementación interna)
│       │   │   ├── DispatcherInterfaces.cs # IRequest, IRequestHandler, IDispatcher
│       │   │   └── NativeDispatcher.cs     # Implementación basada en IServiceProvider
│       │   └── Validation/                 # Pipeline de validación funcional nativo
│       │       └── IValidator.cs           # Contrato y lógica de validación
│       ├── DTOs/                           # Data Transfer Objects canónicos
│       │   └── ApplicationDtos.cs          # AuthResponseDto, CourseDto, StudentDto, etc.
│       ├── Features/                       # Casos de uso segmentados por vertical/comando/consulta
│       │   ├── Admin/                      # Comandos y Queries administrativos
│       │   │   ├── Commands/Create/        # AdminCreateCourse, AdminCreateProfessor
│       │   │   └── Commands/Delete/        # AdminDeleteCourse, AdminDeleteProfessor
│       │   ├── Auth/                       # Registro de estudiantes y autenticación
│       │   │   └── Commands/               # LoginCommand, RegisterStudentCommand
│       │   ├── Catalog/                    # Consultas de catálogo curricular y estudiantes
│       │   │   └── Queries/                # GetCoursesQuery, GetPublicStudentsQuery, GetStudentProfileQuery
│       │   └── Enrollments/                # Inscripción y desinscripción de asignaturas
│       │       ├── Commands/               # EnrollCourseCommand, DropCourseCommand
│       │       └── Queries/                # GetMyEnrollmentsQuery, GetCourseClassmatesQuery
│       ├── Ports/                          # Contratos de salida (Outbound Ports)
│       │   ├── IUserRepository.cs
│       │   ├── IStudentRepository.cs
│       │   ├── ICourseRepository.cs
│       │   ├── IProfessorRepository.cs
│       │   ├── IEnrollmentRepository.cs
│       │   ├── IUnitOfWork.cs
│       │   ├── IPasswordHasher.cs
│       │   └── ITokenService.cs
│       └── Extension/
│           └── DependencyInjection.cs      # Registro de handlers y validadores en DI
├── Adapters/
│   ├── Infrastructure/                     # Adaptadores secundarios (Salida / Driven)
│   │   ├── Persistence/                    # EF Core 8 DbContext, inicializadores y repositorios
│   │   │   ├── ApplicationDbContext.cs     # DbContext con QueryFilters para Soft-Delete
│   │   │   ├── DbInitializer.cs            # Semillado canónico idempotente (5 profesores, 10 cursos, admin)
│   │   │   ├── Configurations/             # Fluent API T-SQL (Índices únicos filtrados, FKs)
│   │   │   └── Repositories/               # Implementaciones de repositorios y UnitOfWork
│   │   ├── Security/                       # Implementaciones criptográficas y tokens
│   │   │   └── SecurityServices.cs         # JwtTokenService, Pbkdf2PasswordHasher
│   │   └── Extension/
│   │       └── DependencyInjection.cs      # Configuración de base de datos (SQL Server / InMemory)
│   └── Api/                                # Adaptador primario (Entrada / Driving)
│       ├── Controllers/                    # Controladores REST delgados heredados de BaseApiController
│       │   ├── BaseApiController.cs        # Mapeo automático de Result<T> a HTTP Status Codes
│       │   ├── AuthController.cs           # /api/v1/auth
│       │   ├── CoursesController.cs        # /api/v1/courses
│       │   ├── StudentsController.cs       # /api/v1/students
│       │   └── AdminController.cs          # /api/v1/admin
│       ├── Middleware/                     # Manejo centralizado de fallos
│       │   └── ProblemDetailsExceptionMiddleware.cs # RFC 7807 para excepciones no controladas
│       └── Program.cs                      # Configuración de pipeline HTTP, CORS, Swagger y Auth
└── tests/
    ├── Domain.UnitTests/                   # Pruebas unitarias de especificaciones e invariantes
    └── Application.UnitTests/              # Pruebas unitarias de comandos, queries y validadores
```

---

### 2.2 Mediador Nativo y Pipeline de Validación (.NET)

Para cumplir con el principio de independencia y cero dependencias de terceros innecesarias, la aplicación utiliza un despachador CQRS interno ligero:

```csharp
public interface IRequest<TResponse> { }

public interface IRequestHandler<in TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}

public interface IDispatcher
{
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}
```

#### Pipeline de Validación Integrado
Antes de invocar el handler correspondiente, el `NativeDispatcher` resuelve los validadores `IValidator<TRequest>` registrados en el contenedor de inyección de dependencias. Si alguna regla de validación de entrada falla, el despachador retorna de forma inmediata un `Result<T>.Failure(ErrorType.Validation, mensaje)` sin llegar a ejecutar la lógica del caso de uso.

---

### 2.3 Manejo de Errores: Result Pattern & RFC 7807

El sistema no utiliza excepciones para el control de flujo de negocio esperado. En su lugar, utiliza un contenedor funcional canónico `Result<T>`:

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public ErrorType ErrorType { get; }
}
```

#### Mapeo HTTP Centralizado en `BaseApiController`:
* `Result.Success(data)` $\to$ `200 OK` (o `201 Created` en registros de inscripción).
* `Result.Failure(ErrorType.Validation, error)` $\to$ `400 Bad Request`.
* `Result.Failure(ErrorType.Unauthorized, error)` $\to$ `401 Unauthorized`.
* `Result.Failure(ErrorType.Forbidden, error)` $\to$ `403 Forbidden`.
* `Result.Failure(ErrorType.NotFound, error)` $\to$ `404 Not Found`.
* `Result.Failure(ErrorType.Conflict, error)` $\to$ `409 Conflict`.

Excepciones no controladas (errores de infraestructura inesperados) son interceptadas por `ProblemDetailsExceptionMiddleware`, generando respuestas estandarizadas bajo **RFC 7807 (Problem Details for HTTP APIs)** con status `500 Internal Server Error`.

---

### 2.4 Specification Pattern en Dominio

Toda regla de compatibilidad o restricción académica se encapsula mediante el patrón de especificación:

```csharp
public interface ISpecification<in T>
{
    bool IsSatisfiedBy(T entity);
    string ErrorMessage { get; }
}
```

La entidad candidata `EnrollmentCandidate` encapsula la carga académica actual del estudiante y la materia solicitada.

Especificaciones de Dominio implementadas:
1. `MaxCoursesSpecification`: Valida que el estudiante tenga menos de 3 materias activas (máximo 9 créditos).
2. `UniqueProfessorSpecification`: Valida que el estudiante no tenga inscrita otra materia con el mismo docente asignado.
3. `NotAlreadyEnrolledSpecification`: Verifica que la materia no haya sido inscrita previamente por el estudiante.

---

## 3. Frontend: Angular 21 Zoneless con Standalone Components

### 3.1 Estructura del Proyecto Frontend

El frontend adopta una arquitectura modular orientada a dominios (*Feature-Driven Architecture*), con separación estricta de archivos `.ts`, `.html` y `.css` por cada componente:

```text
src/frontend/src/
├── app/
│   ├── core/                                # Núcleo transversal de la aplicación (Singleton)
│   │   ├── guards/                          # Guardas funcionales de enrutamiento (RBAC)
│   │   │   └── auth.guard.ts                # studentGuard, adminGuard, homeGuard
│   │   ├── interceptors/                    # Interceptores HTTP funcionales
│   │   │   ├── auth.interceptor.ts          # Inyección de token JWT Bearer
│   │   │   └── api-envelope.interceptor.ts  # Desempaquetado automático de Result<T>
│   │   ├── models/                          # Interfaces TypeScript modulares
│   │   │   ├── auth.models.ts               # LoginRequest, RegisterRequest, AuthResponse
│   │   │   ├── course.models.ts             # Course, CreateCourseRequest
│   │   │   ├── enrollment.models.ts         # EnrollmentSummary, ClassmateDto
│   │   │   ├── student.models.ts            # StudentProfile, PublicStudent
│   │   │   └── index.ts                     # Barrel export centralizado
│   │   └── services/                        # Servicios de estado reactivo con Signals
│   │       ├── auth.service.ts              # Gestión reactiva de sesión, token y roles
│   │       └── academic.service.ts          # Gestión de catálogo, materias y compañeros
│   ├── features/                            # Módulos funcionales de negocio
│   │   ├── admin/                           # Panel administrativo de cursos y profesores
│   │   │   ├── admin.component.ts
│   │   │   ├── admin.component.html
│   │   │   └── admin.component.css
│   │   ├── auth/                            # Flujos de autenticación
│   │   │   ├── login/                       # LoginComponent (.ts, .html, .css)
│   │   │   └── register/                    # RegisterComponent (.ts, .html, .css)
│   │   ├── catalog/                         # Catálogo de materias para selección
│   │   │   ├── catalog.component.ts
│   │   │   ├── catalog.component.html
│   │   │   └── catalog.component.css
│   │   ├── enrollments/                     # Vista "Mis Materias" y compañeros
│   │   │   ├── my-enrollments.component.ts
│   │   │   ├── my-enrollments.component.html
│   │   │   └── my-enrollments.component.css
│   │   └── students/                        # Directorio público de estudiantes
│   │       ├── students.component.ts
│   │       ├── students.component.html
│   │       └── students.component.css
│   ├── shared/                              # Componentes visuales reutilizables
│   │   └── navbar/                          # Barra de navegación principal
│   │       ├── navbar.component.ts
│   │       ├── navbar.component.html
│   │       └── navbar.component.css
│   ├── app.config.ts                        # Configuración Zoneless, Router y HttpClient
│   └── app.routes.ts                        # Definición de rutas protegidas con RBAC
├── environments/                            # Configuración de entornos centralizada
│   ├── environment.ts                       # Producción (URL de API productiva)
│   └── environment.development.ts           # Desarrollo (https://localhost:7210/api/v1)
└── styles.css                               # Sistema de diseño y clases utilitarias DRY
```

---

### 3.2 Arquitectura Reactiva Zoneless y Signals

En `app.config.ts`, la aplicación configura `provideZonelessChangeDetection()` eliminando la sobrecarga de `zone.js`. El estado se gestiona de forma determinista mediante **Angular Signals**:

* **AuthService (`auth.service.ts`):**
  * `currentUser = signal<UserSession | null>(...)`
  * `token = signal<string | null>(...)`
  * `isAuthenticated = computed(() => !!this.token())`
  * `isStudent = computed(() => this.currentUser()?.role === 'Estudiante')`
  * `isAdmin = computed(() => this.currentUser()?.role === 'Administrador')`

* **AcademicService (`academic.service.ts`):**
  * `courses = signal<Course[]>([])`
  * `myEnrollments = signal<EnrollmentSummary | null>(null)`
  * `publicStudents = signal<PublicStudent[]>([])`

* **Reglas Reactivas en Tiempo Real:**
  * En `catalog.component.ts`, las materias ya inscritas o con docentes coincidentes se calculan en memoria mediante Signals derivados:
    - `isEnrolled(courseId)`: Bloquea visualmente materias ya seleccionadas.
    - `hasProfessorConflict(professorId)`: Identifica colisiones de profesor en tiempo real antes del envío al backend, deshabilitando botones y mostrando advertencias claras al usuario.
    - Contador reactivo `0/9 CR` (máximo 3 materias) visible tanto en la tarjeta como en el navbar.

---

### 3.3 Segregación Estricta de Roles (RBAC)

Para preservar la coherencia del dominio académico, los usuarios con rol `Administrador` no participan en la inscripción de materias ni visualizan el catálogo de alumnos:

1. **A Nivel de Interfaz:**
   * En `navbar.component.html`, los enlaces de `Catálogo` y `Mis Materias` están encapsulados bajo directivas `@if (authService.isStudent())`.
2. **A Nivel de Enrutamiento:**
   * `studentGuard`: Bloquea el acceso a `/catalog` y `/my-enrollments` si el usuario es `Administrador`, redirigiéndolo a `/admin`.
   * `adminGuard`: Protege la ruta `/admin` permitiendo el acceso exclusivamente a usuarios administradores.
   * `homeGuard`: Redirige automáticamente la raíz `/` a `/admin` (para administradores) o a `/catalog` (para estudiantes).

---

### 3.4 Desempaquetado Funcional con `apiEnvelopeInterceptor`

El frontend procesa las respuestas del backend mediante un interceptor funcional que examina la estructura `Result<T>`:
* Si la respuesta contiene `{ isSuccess: true, value: ... }`, el interceptor extrae y entrega directamente el objeto `value` a los componentes y servicios, manteniendo el código de los consumidores limpio y fuertemente tipado.
* Si `{ isSuccess: false }`, el interceptor propaga el error como un fallo de Observable con el mensaje descriptivo proveniente del backend.

---

## 4. Esquema de Seguridad y Transporte

* **Autenticación:** Tokens JWT firmados con HMAC-SHA256 y validados mediante middleware nativo de ASP.NET Core.
* **Claims:** `sub` (Identificador del usuario), `email`, `name`, `role` (`Estudiante` | `Administrador`).
* **Políticas de Autorización:**
  * `RequireAdmin`: Aplicado en `AdminController` para creación y eliminación de asignaturas y docentes.
  * `RequireStudent`: Aplicado en endpoints de matrícula (`POST /api/v1/courses/enroll`, `POST /api/v1/courses/drop`, `GET /api/v1/courses/my-enrollments`).
* **Protección CORS:** Política restrictiva configurada en el backend permitiendo peticiones seguras desde los orígenes del frontend autorizados.
* **Criptografía de Contraseñas:** Algoritmo seguro PBKDF2 con salt criptográfico de 128 bits e iteraciones computacionales seguras en `Pbkdf2PasswordHasher`.
