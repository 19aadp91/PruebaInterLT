# Especificación de Arquitectura de Software (`architecture-spec.md`)

## 1. Identidad y Principios
* **Estilo Arquitectónico:** Arquitectura Hexagonal (Puertos y Adaptadores) para Backend; Arquitectura Basada en Componentes Standalone y Reactividad con Signals para Frontend.
* **Patrón de Despacho:** Mediador Nativo Interno (`IDispatcher` / `IRequestHandler<T, R>`), sin paquetes de terceros como MediatR.
* **Patrón de Negocio en Dominio:** Specification Pattern (`ISpecification<T>`) para la evaluación pura y desacoplada de reglas e invariantes de inscripción.
* **Fuente Canónica:** `openapi.yaml` gobierna con tipado estricto los contratos de transporte tanto para C# como para TypeScript.

---

## 2. Backend: Arquitectura Hexagonal (.NET 8/9)

### 2.1 Descomposición en Capas y Puertos

```text
src/backend/
├── Core/
│   ├── Domain/                      # Lógica pura del negocio
│   │   ├── Common/                  # BaseEntity, ValueObject, ISpecification<T>
│   │   ├── Entities/                # Student, Course, Professor, Enrollment, User
│   │   ├── Exceptions/              # DomainValidationException, ConflictException
│   │   └── Specifications/          # Reglas del negocio encapsuladas
│   │       ├── MaxCoursesSpecification.cs
│   │       ├── UniqueProfessorSpecification.cs
│   │       └── NotAlreadyEnrolledSpecification.cs
│   └── Application/                 # Casos de uso y orquestación
│       ├── Common/
│       │   └── Dispatcher/          # MEDIADOR NATIVO (Interfaces e implementación interna)
│       │       ├── IRequest.cs
│       │       ├── IRequestHandler.cs
│       │       └── IDispatcher.cs
│       ├── Features/
│       │   ├── Auth/Commands/       # RegisterStudent, Login
│       │   ├── Enrollments/Commands/# EnrollCourse, DropCourse
│       │   ├── Enrollments/Queries/ # GetMyEnrollments, GetCourseClassmates
│       │   └── Catalog/Queries/     # GetCourses, GetPublicStudents
│       └── Ports/                   # Interfaces de salida requeridas
│           ├── Repositories/        # IStudentRepository, ICourseRepository, etc.
│           ├── Security/            # IPasswordHasher, ITokenService
│           └── IUnitOfWork.cs
├── Adapters/
│   ├── Infrastructure/              # Adaptadores secundarios (Salida)
│   │   ├── Persistence/             # ApplicationDbContext, EF Core Configurations
│   │   │   ├── Configurations/      # Fluent API para SQL Server
│   │   │   └── Repositories/        # Implementaciones concretas de puertos
│   │   └── Security/                # JwtTokenService, Pbkdf2PasswordHasher
│   └── Api/                         # Adaptador primario (Entrada / Transporte)
│       ├── Controllers/             # Controladores delgados (delegan a IDispatcher)
│       ├── Middleware/              # ProblemDetailsExceptionMiddleware (RFC 7807)
│       └── Program.cs               # Registro DI y tubería HTTP
└── tests/
    ├── Domain.UnitTests/            # Pruebas de especificaciones de dominio (>=85%)
    └── Application.UnitTests/       # Pruebas de handlers y casos de uso (>=85%)
```

---

### 2.2 Mediador Nativo (.NET)
Para cumplir con el principio 1 de la Constitución (cero dependencias externas no autorizadas), se implementa un despachador nativo ligero:

```csharp
// Abstracciones nativas de mediador
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

La implementación del despachador resuelve el handler en tiempo de ejecución utilizando directamente `IServiceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>()`.

---

### 2.3 Specification Pattern en Dominio
Toda regla de inscripción se modela como una especificación evaluable:

```csharp
public interface ISpecification<in T>
{
    bool IsSatisfiedBy(T entity);
    string ErrorMessage { get; }
}
```

Especificaciones de Dominio obligatorias:
1. `MaxCoursesSpecification`: Valida que el estudiante tenga menos de 3 asignaturas activas antes de inscribir una nueva.
2. `UniqueProfessorSpecification`: Comprueba que el profesor de la materia solicitada no coincida con el profesor de ninguna de las materias actualmente inscritas por el estudiante.
3. `NotAlreadyEnrolledSpecification`: Verifica que la materia no haya sido inscrita previamente por el estudiante.

---

### 2.4 Manejo de Errores RFC 7807
El middleware global intercepta las excepciones y genera el formato normalizado:
* `DomainValidationException` ➔ HTTP `400 Bad Request` con título y detalle del motivo de incompatibilidad.
* `NotFoundException` ➔ HTTP `404 Not Found`.
* `ConflictException` ➔ HTTP `409 Conflict` (para correos/documentos repetidos o materias ya inscritas).
* `UnauthorizedException` ➔ HTTP `401 Unauthorized`.

---

## 3. Frontend: Angular Standalone con Signals

### 3.1 Estructura del Proyecto Frontend
```text
src/app/
├── core/
│   ├── auth/                        # AuthService, AuthGuard, TokenStorage
│   ├── interceptors/                # AuthInterceptor (Bearer), ErrorInterceptor (RFC 7807)
│   └── models/                      # Interfaces TypeScript sincronizadas con openapi.yaml
│       ├── auth.models.ts
│       ├── course.models.ts
│       ├── enrollment.models.ts
│       └── student.models.ts
├── shared/
│   └── components/                  # Navbar, NotificationToast, Modal, LoadingSpinner
└── features/
    ├── auth/                        # LoginComponent, RegisterComponent
    ├── catalog/                     # CourseListComponent (selección reactiva con Signals)
    ├── my-enrollments/              # EnrollmentSummaryComponent, ClassmatesModalComponent
    ├── students/                    # PublicStudentsListComponent (solo nombres)
    └── admin/                       # AdminProfessorsComponent, AdminCoursesComponent
```

### 3.2 Gestión Reactiva con Signals
* El catálogo de materias seleccionadas se maneja mediante:
  * `selectedCourses = signal<CourseDto[]>([])`
  * `selectedCredits = computed(() => this.selectedCourses().length * 3)`
  * `hasMaxCredits = computed(() => this.selectedCourses().length >= 3)`
  * `selectedProfessorIds = computed(() => this.selectedCourses().map(c => c.professor.id))`
* En la plantilla, las tarjetas de asignaturas cuyo profesor ya esté en `selectedProfessorIds()` se muestran visualmente deshabilitadas con la advertencia: *"Profesor ya seleccionado en otra materia"*, previniendo errores de usuario en tiempo real antes de llamar al backend.

---

## 4. Esquema de Seguridad
* **Autenticación:** Tokens JWT firmados con clave simétrica HMAC-SHA256 (mínimo 256 bits).
* **Claims:** `sub` (Id del usuario), `email`, `name`, `role` (`Estudiante` o `Administrador`).
* **Políticas:**
  * `RequireAdmin`: Solo usuarios con claim `role: 'Administrador'`.
  * `RequireStudent`: Solo usuarios con claim `role: 'Estudiante'`.
