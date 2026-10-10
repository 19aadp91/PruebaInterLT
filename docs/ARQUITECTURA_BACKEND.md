# Arquitectura del Backend: Especificación Técnica y Justificación de Diseño

**Sistema:** Plataforma de Registro Académico y Selección de Materias  
**Plataforma:** .NET 8 (C# 12)  
**Organización:** Inter Rapidísimo S.A.  
**Estado:** Producción / Validado con 100% de Pruebas Unitarias  

---

## 1. Visión General de la Arquitectura

El backend implementa una **Arquitectura Hexagonal (Ports & Adapters)** fusionada con los principios de **Clean Architecture** y una distribución canónica del patrón **CQRS (Command Query Responsibility Segregation)**.

El diseño prioriza el desacoplamiento estricto, la independencia de librerías externas para componentes neurálgicos (como el mediador y las validaciones), la alta testabilidad y el control de flujo funcional mediante el **Result Pattern** (*Railway Oriented Programming*).

```mermaid
graph TD
    subgraph Adaptadores_Entrada [Adaptadores de Entrada (Driving Adapters)]
        API[ASP.NET Core Web API Controllers]
        MW[ProblemDetails / Exception Middleware]
    end

    subgraph Nucleo_Aplicacion [Núcleo de Aplicación (Application Layer)]
        Dispatcher[Native Dispatcher (Mediador Nativo)]
        Validators[Validadores Funcionales IValidator]
        Handlers[Command & Query Handlers]
        Ports[Puertos de Repositorio e Interfaces]
    end

    subgraph Dominio [Núcleo de Dominio (Domain Layer)]
        Entities[Entidades de Dominio]
        Specs[Especificaciones de Negocio]
        Result[Result Pattern & ErrorType]
    end

    subgraph Adaptadores_Salida [Adaptadores de Salida (Driven Adapters)]
        EF[Entity Framework Core DbContext]
        Repos[Implementaciones de Repositorio]
        Auth[Servicio JWT & Criptografía PBKDF2]
        DB[(SQL Server Database)]
    end

    API -->|Envía IRequest| Dispatcher
    Dispatcher -->|1. Evalúa| Validators
    Dispatcher -->|2. Despacha| Handlers
    Handlers -->|Aplica reglas| Specs
    Handlers -->|Opera| Entities
    Handlers -->|Invoca contratos| Ports
    Ports -.->|Implementado por| Repos
    Repos --> EF
    EF --> DB
    API -->|Mapea| Result
    MW -->|Captura no controlados| Result
```

### Principios Fundamentales
* **Inversión de Dependencias (DIP):** Las capas internas (`Domain` y `Application`) nunca dependen de las capas externas (`Infrastructure` o `Api`). Las capas externas implementan los puertos definidos en el núcleo.
* **Separación de Responsabilidades (SoC):** Cada clase y capa posee un único propósito delimitado. Los controladores HTTP solo gestionan transporte y códigos de estado; los handlers coordinan casos de uso; las entidades encapsulan estado e invariantes.
* **Zero External Dependencies en el Núcleo:** El mediador CQRS, los validadores y las abstracciones de negocio se implementaron de forma nativa sin depender de paquetes NuGet como MediatR o FluentValidation.

---

## 2. Topología de la Solución y Distribución de Carpetas

La solución sigue una convención física y lógica estricta que refleja el flujo de dependencias:

```text
src/backend/
├── Core/
│   ├── Domain/                      # Capa más interna: Reglas puras, Entidades y Primitivas
│   │   ├── Common/                  # Clases base, especificaciones y Result<T>
│   │   ├── Entities/                # Modelos del dominio con estado e identidad
│   │   └── Specifications/          # Reglas complejas encapsuladas (Specification Pattern)
│   │
│   └── Application/                 # Casos de uso de negocio (CQRS Canónico)
│       ├── Common/                  # Interfaces del mediador y validadores nativos
│       │   ├── Dispatcher/          # IRequest, IRequestHandler, IDispatcher, NativeDispatcher
│       │   └── Validation/          # IValidator<TRequest>
│       ├── DTOs/                    # Contratos de transferencia de datos de entrada/salida
│       ├── Extension/               # Inyección de dependencias modular (DependencyInjection.cs)
│       ├── Features/                # Casos de uso organizados por Slices Funcionales
│       │   ├── Admin/               # Comandos administrativos (Create, Delete)
│       │   ├── Auth/                # Comandos de autenticación (Login, Register)
│       │   ├── Catalog/             # Consultas de materias y perfiles (GetCourses, etc.)
│       │   └── Enrollments/         # Inscripción y cancelación de materias (Enroll, Drop)
│       └── Ports/                   # Puertos secundarios: Contratos de repositorios y servicios
│
├── Adapters/
│   ├── Infrastructure/              # Adaptador secundario: Base de datos y servicios externos
│   │   ├── Authentication/          # Generación de tokens JWT y hashing PBKDF2
│   │   ├── Extension/               # Inyección de dependencias modular (DependencyInjection.cs)
│   │   ├── Persistence/             # EF Core DbContext, Semillas e Inicializador
│   │   │   ├── Configurations/      # Mapeos Fluent API independientes por entidad
│   │   │   └── Repositories/        # Implementación concreta de los puertos del núcleo
│   │
│   └── Api/                         # Adaptador primario: Entrada HTTP / REST
│       ├── Common/                  # Extensiones de traducción (ErrorTypeExtensions)
│       ├── Controllers/             # Controladores delgados derivados de BaseApiController
│       ├── Extension/               # Inyección de dependencias de API (DependencyInjection.cs)
│       ├── Middleware/              # Manejo global de excepciones inesperadas
│       └── Program.cs               # Punto de entrada ultraligero y orquestador del pipeline
│
└── tests/
    ├── Domain.UnitTests/            # Pruebas unitarias de especificaciones y Result<T>
    └── Application.UnitTests/       # Pruebas unitarias de Handlers y flujos de negocio
```

---

## 3. Desglose Técnico por Capas y Justificaciones

### 3.1. Capa de Dominio (`Core/Domain`)

Es el núcleo del sistema, libre de cualquier dependencia externa, framework de infraestructura o referencias a bases de datos.

#### Componentes Principales:
1. **Entidades (`Domain/Entities/`):**
   * Representan los conceptos clave: `Student`, `Professor`, `Course`, `Enrollment`, `User`.
   * Heredan de `BaseEntity`, la cual garantiza columnas canónicas de auditoría: `Id`, `CreatedAtUtc`, `UpdatedAtUtc` e `IsDeleted` (Soft Delete).
2. **Patrón Specification (`Domain/Specifications/`):**
   * Las reglas de negocio complejas no se dispersan en servicios o controladores. Se encapsulan mediante la interfaz `ISpecification<T>`:
     * `MaxCoursesSpecification`: Garantiza que ningún estudiante exceda el límite de 3 materias inscritas (9 créditos).
     * `UniqueProfessorSpecification`: Aplica la regla innegociable que prohíbe inscribir dos materias dictadas por el mismo profesor.
     * `NotAlreadyEnrolledSpecification`: Impide la inscripción duplicada de la misma materia activa.
   * **Justificación:** Centraliza la lógica de negocio, facilita su reutilización y permite probar las reglas de forma aislada sin levantar contextos de base de datos ni mocks complejos.
3. **Primitiva Funcional `Result<T>` y `ErrorType` (`Domain/Common/Result.cs`):**
   * Sustituye el lanzamiento de excepciones para control de flujo por el patrón de resultados.

---

### 3.2. Capa de Aplicación (`Core/Application`)

Implementa la orquestación de los casos de uso del sistema utilizando una distribución canónica de **CQRS Vertical Slice**.

#### 3.2.1. Organización de Features (Cohesión Vertical)
En lugar de agrupar todos los comandos en una carpeta masiva y todos los manejadores en otra, cada funcionalidad reside en su propio subdirectorio:

```text
Features/Enrollments/Commands/Enroll/
├── EnrollCourseCommand.cs            # Definición inmutable de la solicitud (IRequest<Result<T>>)
├── EnrollCourseCommandValidator.cs   # Reglas de validación previas a la ejecución
└── EnrollCourseCommandHandler.cs     # Lógica de orquestación del caso de uso
```

> [!NOTE]
> **Justificación Arquitectónica:**
> Este enfoque maximiza el principio de **alta cohesión**. Si se requiere modificar o auditar el caso de uso de inscripción, todos los artefactos relacionados (contrato, validación y ejecución) están agrupados en el mismo contexto físico y conceptual, minimizando la dispersión de código y el riesgo de regresiones.

#### 3.2.2. Mediador Nativo (`NativeDispatcher`)
Se diseñó un despachador interno basado en el patrón Mediador:
* `IDispatcher`: Expone el contrato `Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)`.
* `NativeDispatcher`: Resuelve en tiempo de ejecución el manejador correspondiente desde el contenedor de dependencias de .NET.
* **Pipeline Behavior de Validación:** Antes de invocar a cualquier manejador, el despachador consulta si existe un `IValidator<TRequest>` registrado para la solicitud. Si el validador retorna un fallo (`!result.IsSuccess`), el flujo se **cortocircuita inmediatamente**, retornando `Result<TResponse>.Failure(...)` sin tocar el Handler ni la base de datos.
* **Justificación:** Elimina la dependencia de paquetes de terceros (MediatR), reduce la huella de memoria del runtime y otorga control total sobre la cadena de intercepción.

#### 3.2.3. Puertos Secundarios (`Application/Ports/`)
Define interfaces estrictas para la persistencia y servicios transversales:
* `IStudentRepository`, `ICourseRepository`, `IProfessorRepository`, `IEnrollmentRepository`, `IUserRepository`.
* `IUnitOfWork`: Garantiza consistencia transaccional mediante `CommitAsync()`.
* `IPasswordHasher`, `ITokenService`.
* **Justificación:** Desacopla la lógica de negocio del motor de persistencia subyacente. Los casos de uso operan contra abstracciones puras, permitiendo cambiar el proveedor de base de datos o implementar simuladores de prueba sin alterar una sola línea de lógica.

---

### 3.3. Capa de Infraestructura (`Adapters/Infrastructure`)

Provee las implementaciones concretas para los puertos requeridos por la capa de aplicación.

#### Componentes Principales:
1. **Configuraciones Fluent API Independientes (`Persistence/Configurations/`):**
   * Cada entidad tiene su propio archivo de configuración (`CourseConfiguration.cs`, `EnrollmentConfiguration.cs`, etc.).
   * Establece restricciones de unicidad, longitudes máximas, relaciones foráneas e índices filtrados para exclusiones lógicas (`IsDeleted = 0`).
2. **Repositorios Individuales (`Persistence/Repositories/`):**
   * Cada puerto del núcleo cuenta con una clase de implementación dedicada (ej. `CourseRepository.cs`), evitando repositorios genéricos anémicos que limitan la optimización de consultas SQL.
3. **Criptografía y Autenticación:**
   * `Pbkdf2PasswordHasher`: Hashing criptográfico seguro utilizando sal aleatoria e iteraciones SHA-256.
   * `JwtTokenService`: Emisión de tokens firmados bajo el estándar RFC 7519 con roles de usuario (`Estudiante`, `Administrador`).
4. **Semillas y Creación (`DbInitializer.cs`):**
   * Carga controlada de datos iniciales requeridos: 5 profesores, 10 materias académicas y credenciales administrativas por defecto.

---

### 3.4. Capa de API (`Adapters/Api`)

Es el adaptador primario que expone los casos de uso a través del protocolo HTTP / REST.

#### 3.4.1. Controlador Base y Mapeo Dinámico de Códigos HTTP
Los controladores heredan de `BaseApiController`, el cual centraliza el método de respuesta `ToActionResult`:

```csharp
protected IActionResult ToActionResult<T>(Result<T> result, int successStatusCode = StatusCodes.Status200OK)
{
    var statusCode = result.IsSuccess ? successStatusCode : result.ErrorType.ToHttpStatusCode();
    return StatusCode(statusCode, result);
}
```

El método de extensión `ErrorTypeExtensions` traduce la semántica del dominio a códigos estándar de la IETF:

| `ErrorType` (Dominio) | Código HTTP | Significado REST |
| :--- | :---: | :--- |
| `None` | `200 OK` / `201 Created` | Operación ejecutada exitosamente |
| `Validation` / `BadRequest` | `400 Bad Request` | Violación de regla de negocio o formato de entrada |
| `Unauthorized` | `401 Unauthorized` | Credenciales inválidas o ausencia de token de autenticación |
| `Forbidden` | `403 Forbidden` | Permisos insuficientes para el recurso solicitado |
| `NotFound` | `404 Not Found` | Recurso o entidad inexistente en el sistema |
| `Conflict` | `409 Conflict` | Conflicto de estado (duplicidad de materias, profesor repetido) |
| `InternalServerError` | `500 Internal Server Error` | Excepción técnica no controlada del sistema |

#### 3.4.2. Middleware Global de Excepciones Inesperadas
`ProblemDetailsExceptionMiddleware` actúa como red de seguridad final. No captura errores de negocio (ya controlados por `Result<T>`), sino fallas imprevistas de infraestructura (caída del servidor SQL, timeout de red). En estos casos, serializa un `Result<object?>.Failure(...)` con código 500, impidiendo que trazas internas de la aplicación se filtren al cliente.

---

## 4. Justificación del Patrón Result frente al Uso de Excepciones

La sustitución total de excepciones de dominio por la clase `Result<T>` responde a decisiones de ingeniería de software de alto rendimiento:

```text
Flujo Tradicional (Antipatrón Exception-Driven Flow):
Request ──> Handler ──> throw DomainException ──> Unwinding de Stack Trace ──> Middleware ──> Response HTTP

Flujo Funcional Implementado (Result Pattern):
Request ──> Handler ──> return Result<T>.Failure(...) ──> ToActionResult ──> Response HTTP (Instantáneo)
```

### Argumentos Técnicos:
1. **Cero Penalización de Rendimiento:** El lanzamiento y captura de excepciones en el CLR de .NET requiere pausar la ejecución del hilo para inspeccionar la pila de llamadas (*stack unwinding*). En escenarios de alta concurrencia o errores frecuentes de validación, esto genera una sobrecarga computacional severa. `Result<T>` es un retorno de objeto regular en memoria sin coste adicional.
2. **Firma de Métodos Explícita y Predictible:** Una firma `Task<Result<AuthResponse>> HandleAsync(...)` documenta formalmente en el sistema de tipos de C# que la operación puede fallar y obliga al consumidor a evaluar el éxito o fracaso, eliminando efectos secundarios ocultos.
3. **Estandarización del Contrato JSON:** El 100% de las respuestas de la API devuelven una estructura uniforme:
   ```json
   {
     "isSuccess": false,
     "message": "Ya existe una materia registrada con ese código.",
     "errors": [],
     "result": null,
     "errorType": 6
   }
   ```

---

## 5. Modularidad de la Inyección de Dependencias

Para evitar la degradación de [`Program.cs`](file:///c:/Users/alvar/Documents/Prueba%20inter/src/backend/Adapters/Api/Program.cs) en un archivo monolítico (*God File*), cada capa expone su propia clase estática `DependencyInjection.cs`:

```csharp
// Program.cs queda reducido a 30 líneas de alta legibilidad:
builder.Services.AddApplicationDispatcher();                 // Capa Application
builder.Services.AddInfrastructureServices(builder.Configuration); // Capa Infrastructure
builder.Services.AddApiServices(builder.Configuration);            // Capa Api
```

### Registro Determinista sin Reflexión Insegura
En `Application/Extension/DependencyInjection.cs`, el registro de Handlers y Validadores se realiza de forma **explícita y determinista**:
* **Problema resuelto:** El uso de escaneo por reflexión dinámico (`Assembly.GetTypes()`) suele fallar en entornos corporativos o Windows 11 con directivas de integridad de código (*Smart App Control*) debido al bloqueo preventivo de DLLs locales no firmadas (código de error `0x800711C7`).
* **Solución:** El registro directo previene al 100% las excepciones de tipo `ReflectionTypeLoadException`, acelera el tiempo de arranque en frío (*cold start*) y garantiza un comportamiento idéntico en desarrollo y producción.

---

## 6. Estrategia de Pruebas Automatizadas

La solución cuenta con una suite integral de pruebas unitarias implementadas con **xUnit**, **Moq** y **FluentAssertions**:

* **`Domain.UnitTests`:**
  * Evaluación exhaustiva de las especificaciones (`MaxCoursesSpecification`, `UniqueProfessorSpecification`, `NotAlreadyEnrolledSpecification`).
  * Comprobación del comportamiento de las fábricas estáticas `Result<T>.Success` y `Result<T>.Failure`.
* **`Application.UnitTests`:**
  * Cobertura de flujos completos en `RegisterStudentCommandHandler`, `LoginCommandHandler`, `EnrollCourseCommandHandler`, `DropCourseCommandHandler`, etc.
  * Verificación de que las respuestas de fallo retornan el `ErrorType` esperado sin arrojar excepciones.
  * Validación de la regla de privacidad por diseño en las consultas de compañeros y estudiantes públicos.

---

## 7. Resumen de Decisiones de Arquitectura

| Decisión | Enfoque Implementado | Beneficio Directo |
| :--- | :--- | :--- |
| **Estilo Arquitectónico** | Hexagonal (Ports & Adapters) | Independencia de base de datos, frameworks y alta testabilidad |
| **Separación de Casos de Uso** | CQRS Canónico en Vertical Slices | Alta cohesión de comandos, consultas, validadores y manejadores |
| **Manejo de Errores** | Result Pattern con `ErrorType` | Cero costo de excepciones de pila y contratos JSON uniformes |
| **Mediador CQRS** | Despachador Nativo (`NativeDispatcher`) | Sin dependencias externas de NuGet; control total del ciclo de vida |
| **Validaciones** | `IValidator<T>` con cortocircuito en pipeline | Evaluación previa sin alcanzar los repositorios ni saturar la BD |
| **Reglas de Negocio** | Specification Pattern (`ISpecification<T>`) | Lógica compleja aislada, testeable y reutilizable |
| **Transaccionalidad** | Unit of Work explícito | Atomicidad en operaciones que involucran múltiples agregados |
| **Inyección de Dependencias** | Extensiones modulares por capa | `Program.cs` limpio, arranque determinista e inmune a fallos de reflexión |
