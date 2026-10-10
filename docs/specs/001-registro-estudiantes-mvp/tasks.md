# Desglose de Tareas de Implementación (`tasks.md`)

## 1. Resumen
* **Especificación:** [spec.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/spec.md)
* **Plan Técnico:** [plan.md](file:///C:/Users/alvar/Documents/Prueba%20inter/docs/specs/001-registro-estudiantes-mvp/plan.md)
* **Total de Tareas:** 22 tareas atómicas en orden de capas y dependencias.

---

## 2. Backlog de Tareas

### Capa 1: Dominio Hexagonal
- [ ] **T-01** — Modelar entidades puras del dominio (`User`, `Student`, `Professor`, `Course`, `Enrollment`) y excepciones específicas.
  - Cubre: RF-1, RF-3, RF-4
  - Depende de: ninguna
  - Hecho cuando: Clases de entidad compiladas en `Domain` sin referencias a frameworks externos.

- [ ] **T-02** — Implementar la abstracción `ISpecification<T>` y la regla `MaxCoursesSpecification`.
  - Cubre: RF-4
  - Depende de: T-01
  - Hecho cuando: `MaxCoursesSpecification` evalúa que el número de materias inscritas sea menor a 3.

- [ ] **T-03** — Crear pruebas unitarias para `MaxCoursesSpecification`.
  - Cubre: RF-4
  - Depende de: T-02
  - Hecho cuando: Tests `MaxCoursesSpecification_WhenUnderThreeCourses_IsSatisfied` y `MaxCoursesSpecification_WhenThreeCoursesAlreadyEnrolled_Fails` en verde.

- [ ] **T-04** — Implementar las especificaciones `UniqueProfessorSpecification` y `NotAlreadyEnrolledSpecification`.
  - Cubre: RF-4
  - Depende de: T-01
  - Hecho cuando: Ambas especificaciones retornan mensajes claros de incompatibilidad al detectar duplicidad de profesor o materia.

- [ ] **T-05** — Crear pruebas unitarias para `UniqueProfessorSpecification` y `NotAlreadyEnrolledSpecification`.
  - Cubre: RF-4
  - Depende de: T-04
  - Hecho cuando: Tests `UniqueProfessorSpecification_*` y `NotAlreadyEnrolledSpecification_*` en verde con cobertura del 100%.

---

### Capa 2: Aplicación y Mediador Nativo
- [ ] **T-06** — Implementar el Despachador/Mediador Nativo (`IRequest<T>`, `IRequestHandler<T, R>`, `IDispatcher`) con el ServiceProvider de .NET.
  - Cubre: RF-1 a RF-8
  - Depende de: T-01
  - Hecho cuando: El despachador envía una petición y resuelve el handler correspondiente sin paquetes externos.

- [ ] **T-07** — Definir los puertos e interfaces de repositorios (`IStudentRepository`, `ICourseRepository`, etc.) y seguridad (`IPasswordHasher`, `ITokenService`).
  - Cubre: RF-1 a RF-8
  - Depende de: T-01
  - Hecho cuando: Todas las interfaces de puertos estén declaradas en `Application/Ports`.

- [ ] **T-08** — Implementar casos de uso de Autenticación (`RegisterStudentCommand`, `LoginCommand`) y sus pruebas unitarias.
  - Cubre: RF-1, RF-2
  - Depende de: T-06, T-07
  - Hecho cuando: `RegisterStudent_WithValidData_ReturnsSuccessAndToken` y `Login_WithValidCredentials_ReturnsJwtToken` en verde.

- [ ] **T-09** — Implementar casos de uso de Inscripción (`EnrollCourseCommand`, `DropCourseCommand`, `GetMyEnrollmentsQuery`) orquestando las especificaciones de dominio.
  - Cubre: RF-4, RF-5
  - Depende de: T-02, T-04, T-06, T-07
  - Hecho cuando: `EnrollCourse_WithValidConditions_*` y `DropCourse_WhenEnrolled_*` pasan pruebas unitarias verificando créditos y profesores.

- [ ] **T-10** — Implementar consultas públicas y de compañeros (`GetCoursesQuery`, `GetPublicStudentsQuery`, `GetCourseClassmatesQuery`) garantizando proyección segura de nombres.
  - Cubre: RF-3, RF-6, RF-7
  - Depende de: T-06, T-07
  - Hecho cuando: Las pruebas verifican que los DTOs de retorno contengan únicamente el nombre y omitan datos sensibles.

- [ ] **T-11** — Implementar casos de uso de Administración para docentes y materias con reglas de bloqueo por inscripciones activas.
  - Cubre: RF-8
  - Depende de: T-06, T-07
  - Hecho cuando: Las pruebas validan que no se puede eliminar un docente o materia con inscripciones vigentes.

---

### Capa 3: Base de Datos y Adaptadores de Infraestructura
- [ ] **T-12** — Crear script T-SQL DDL para SQL Server con tablas, llaves foráneas, índices de rendimiento y datos semilla (5 profesores y 10 materias).
  - Cubre: RF-1, RF-3, RF-4
  - Depende de: T-01
  - Hecho cuando: Script SQL ejecutable de forma idempotente creando el esquema completo y semillas.

- [ ] **T-13** — Implementar `ApplicationDbContext`, configuraciones Fluent API y adaptadores de repositorio con soft-delete universal.
  - Cubre: RF-1 a RF-8
  - Depende de: T-07, T-12
  - Hecho cuando: Los repositorios de persistencia guardan y consultan entidades respetando constraints y auditoría.

- [ ] **T-14** — Implementar adaptadores de seguridad (`Pbkdf2PasswordHasher` con salt e iteraciones y `JwtTokenService`).
  - Cubre: RF-1, RF-2
  - Depende de: T-07
  - Hecho cuando: El hash de contraseñas y la firma/validación de tokens JWT se verifican con tests unitarios.

---

### Capa 4: API HTTP y Transporte
- [ ] **T-15** — Crear controladores delgados (`AuthController`, `StudentsController`, `CoursesController`, `AdminController`) delegando a `IDispatcher`.
  - Cubre: RF-1 a RF-8
  - Depende de: T-08, T-09, T-10, T-11
  - Hecho cuando: Los endpoints coinciden al 100% con las rutas y esquemas declarados en `openapi.yaml`.

- [ ] **T-16** — Implementar Middleware de excepciones RFC 7807 (`ProblemDetailsExceptionMiddleware`) y configurar contenedor de dependencias en `Program.cs`.
  - Cubre: RF-1 a RF-8
  - Depende de: T-15
  - Hecho cuando: Cualquier fallo de negocio o validación devuelve una respuesta HTTP estructurada bajo `ProblemDetails`.

---

### Capa 5: Frontend Angular (Signals & Standalone)
- [ ] **T-17** — Crear modelos e interfaces TypeScript sincronizados con `openapi.yaml`.
  - Cubre: RF-1 a RF-8
  - Depende de: T-15
  - Hecho cuando: Interfaces compiladas con tipado estricto y cero uso de `any`.

- [ ] **T-18** — Implementar servicios de comunicación HTTP (`AuthService`, `StudentService`, `CourseService`) e interceptores funcionales (`AuthInterceptor`, `ErrorInterceptor`).
  - Cubre: RF-1 a RF-8
  - Depende de: T-17
  - Hecho cuando: Los servicios consumen la API inyectando el token Bearer y capturando errores RFC 7807.

- [ ] **T-19** — Implementar vistas de autenticación (`LoginComponent`, `RegisterComponent`) con formularios reactivos y validaciones.
  - Cubre: RF-1, RF-2
  - Depende de: T-18
  - Hecho cuando: El usuario puede registrarse e iniciar sesión visualizando mensajes de error claros.

- [ ] **T-20** — Implementar catálogo interactivo de materias con Signals, bloqueando reactivamente materias con docentes ya seleccionados.
  - Cubre: RF-3, RF-4
  - Depende de: T-18
  - Hecho cuando: Al seleccionar una materia, las materias del mismo profesor se deshabilitan en vivo en la UI.

- [ ] **T-21** — Implementar pantalla de materias inscritas con retiro de materias y modal de compañeros de clase (exponiendo solo nombres).
  - Cubre: RF-5, RF-7
  - Depende de: T-20
  - Hecho cuando: El estudiante puede retirar materias y consultar a sus compañeros de cada asignatura.

- [ ] **T-22** — Implementar lista pública de estudiantes registrados (solo nombres) y panel de administración básico.
  - Cubre: RF-6, RF-8
  - Depende de: T-18
  - Hecho cuando: Se listan los estudiantes registrados respetando la privacidad y el administrador puede gestionar el catálogo.

---

## 3. Matriz de Trazabilidad RF ↔ Tareas

| Requisito Funcional | Tareas que lo Cubren |
| :--- | :--- |
| **RF-1** (Registro en Línea) | T-01, T-07, T-08, T-12, T-13, T-14, T-15, T-16, T-18, T-19 |
| **RF-2** (Autenticación JWT) | T-06, T-07, T-08, T-14, T-15, T-16, T-18, T-19 |
| **RF-3** (Catálogo de Materias) | T-01, T-07, T-10, T-12, T-13, T-15, T-18, T-20 |
| **RF-4** (Inscripción & Profesor Único) | T-02, T-03, T-04, T-05, T-09, T-12, T-13, T-15, T-16, T-20 |
| **RF-5** (Modificación y Retiro) | T-09, T-13, T-15, T-16, T-21 |
| **RF-6** (Lista Pública de Estudiantes) | T-10, T-13, T-15, T-22 |
| **RF-7** (Compañeros de Clase) | T-10, T-13, T-15, T-21 |
| **RF-8** (Gestión Administrativa) | T-11, T-13, T-15, T-22 |

---

## 4. Declaración de Verificación
* Todo RF de la especificación cuenta con al menos dos tareas asociadas (backend y frontend).
* El orden de ejecución respeta rigurosamente el flujo de capas hexagonales sin dependencias circulares.
* Cada regla de negocio (máximo 3 materias, profesor no repetido, materia no duplicada) cuenta con su tarea específica de tests unitarios.
* Cada tarea cuenta con un criterio verificable en "Hecho cuando:".
