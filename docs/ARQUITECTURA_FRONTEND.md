# Arquitectura del Frontend: Especificación Técnica y Justificación de Diseño

**Sistema:** Plataforma de Registro Académico y Selección de Materias  
**Tecnología:** Angular 21 (Node 24 / TypeScript 5.9)  
**Organización:** Inter Rapidísimo S.A.  
**Estado:** Producción / Compilación Validada (0 advertencias, 0 errores)  

---

## 1. Visión General de la Arquitectura

El frontend ha sido diseñado bajo los estándares modernos de **Angular 21**, adoptando una **Arquitectura Reactiva Zoneless** basada en **Angular Signals** y **Standalone Components**. 

El diseño prioriza el desacoplamiento estricto, la eliminación de paquetes redundantes (como `zone.js`), la segregación de roles mediante Guardas Funcionales, el consumo transparente de la envoltura funcional `Result<T>` del backend y una distribución modular de componentes por dominio (*Feature-Driven Architecture*).

```mermaid
graph TD
    subgraph Capa_Presentacion [Capa de Presentación (UI & Features)]
        Nav[NavbarComponent]
        Catalog[CatalogComponent]
        MyEnroll[MyEnrollmentsComponent]
        Students[StudentsComponent]
        Admin[AdminComponent]
        Auth[Login & Register Components]
    end

    subgraph Capa_Enrutamiento [Enrutamiento & Seguridad (RBAC)]
        Router[Angular Router]
        StudentGuard[studentGuard]
        AdminGuard[adminGuard]
        HomeGuard[homeGuard]
    end

    subgraph Capa_Estado_Servicios [Estado Reactivo & Servicios (Core Services)]
        AuthSvc[AuthService (Signals: token, currentUser, role)]
        AcadSvc[AcademicService (Signals: courses, myEnrollments, publicStudents)]
    end

    subgraph Capa_Http_Infraestructura [Capa de Transporte & Interceptores]
        AuthInterceptor[authInterceptor (JWT Bearer Injection)]
        EnvelopeInterceptor[apiEnvelopeInterceptor (Result Pattern Unpacking)]
        HttpClient[Angular HttpClient]
        Env[Environment (Centralized API URLs)]
    end

    subgraph Backend_Api [Backend ASP.NET Core]
        API[(REST API :7210)]
    end

    Router -->|Evalúa Guardas| StudentGuard
    Router -->|Evalúa Guardas| AdminGuard
    Router -->|Evalúa Guardas| HomeGuard
    
    StudentGuard -->|Permite/Redirige| Catalog
    StudentGuard -->|Permite/Redirige| MyEnroll
    AdminGuard -->|Permite/Redirige| Admin
    
    Nav -->|Consume Signals| AuthSvc
    Nav -->|Consume Signals| AcadSvc
    Catalog -->|Consume Signals & Acciones| AcadSvc
    MyEnroll -->|Consume Signals & Acciones| AcadSvc
    Admin -->|Invoca Acciones| AcadSvc
    Auth -->|Autentica| AuthSvc

    AuthSvc --> HttpClient
    AcadSvc --> HttpClient
    HttpClient --> AuthInterceptor
    AuthInterceptor --> EnvelopeInterceptor
    EnvelopeInterceptor --> Env
    Env --> API
```

### Principios Fundamentales
* **Arquitectura Zoneless Nativa:** Se implementa `provideZonelessChangeDetection()`, eliminando por completo la necesidad de `zone.js`. Las actualizaciones del DOM ocurren de manera atómica y eficiente mediante Signals, sin sobrecoste de *monkey-patching* global en microtareas del navegador.
* **Componentes Standalone Puros:** Eliminación definitiva de los antiguos `NgModule`. Cada componente declara explícitamente sus dependencias en `imports`, facilitando el *tree-shaking* y acelerando los tiempos de compilación.
* **Separación Estricta de Interfaz (HTML/CSS/TS):** Cada componente cuenta con sus tres archivos independientes según las directrices oficiales de Angular, garantizando mantenibilidad y claridad.
* **Segregación de Roles (RBAC):** La interfaz y el enrutador discriminan estrictamente entre el rol `Estudiante` y `Administrador`. Un administrador no tiene acceso ni visualización del catálogo ni de la inscripción de asignaturas.

---

## 2. Topología de la Solución y Distribución de Carpetas

La estructura física del proyecto refleja una organización **Modular por Capas y Funcionalidades**:

```text
src/frontend/src/
├── app/
│   ├── core/                                # Núcleo transversal de la aplicación (Singletons y Contratos)
│   │   ├── guards/                          # Guardas funcionales de protección de rutas (RBAC)
│   │   │   └── auth.guard.ts                # studentGuard, adminGuard, homeGuard, authGuard
│   │   │
│   │   ├── interceptors/                    # Pipeline HTTP de salida y entrada
│   │   │   ├── auth.interceptor.ts          # Inyección del token Bearer JWT
│   │   │   └── api-envelope.interceptor.ts  # Desempaquetado del Result<T> del backend
│   │   │
│   │   ├── models/                          # Definición estricta de interfaces y contratos
│   │   │   ├── auth.model.ts                # UserProfileDto, AuthResponse
│   │   │   ├── classmate.model.ts           # ClassmateDto (Políticas de privacidad)
│   │   │   ├── course.model.ts              # CourseDto
│   │   │   ├── enrollment.model.ts          # EnrolledCourseItemDto, StudentEnrollmentSummaryDto
│   │   │   ├── problem-details.model.ts     # ProblemDetails (RFC 7807)
│   │   │   ├── professor.model.ts           # ProfessorDto
│   │   │   ├── result.model.ts              # Result<T> y ErrorType (Railway Oriented Programming)
│   │   │   ├── student.model.ts             # PublicStudentDto, StudentProfileDto
│   │   │   ├── index.ts                     # Barrel export unificado
│   │   │   └── models.ts                    # Reexportación para compatibilidad
│   │   │
│   │   └── services/                        # Servicios de negocio con reactividad basada en Signals
│   │       ├── auth.service.ts              # Sesión, Login, Register, Logout y Signals de usuario
│   │       └── academic.service.ts          # Gestión de catálogo, matrículas y administración
│   │
│   ├── features/                            # Módulos funcionales de la aplicación
│   │   ├── admin/                           # Panel administrativo de docentes y asignaturas
│   │   │   ├── admin.component.ts
│   │   │   ├── admin.component.html
│   │   │   └── admin.component.css
│   │   │
│   │   ├── auth/                            # Autenticación y registro desacoplado por vistas
│   │   │   ├── login/
│   │   │   │   ├── login.component.ts
│   │   │   │   ├── login.component.html
│   │   │   │   └── login.component.css
│   │   │   └── register/
│   │   │       ├── register.component.ts
│   │   │       ├── register.component.html
│   │   │       └── register.component.css
│   │   │
│   │   ├── catalog/                         # Catálogo de 10 materias con validación en tiempo real
│   │   │   ├── catalog.component.ts
│   │   │   ├── catalog.component.html
│   │   │   └── catalog.component.css
│   │   │
│   │   ├── enrollments/                     # Consulta de inscripciones y compañeros de clase
│   │   │   ├── my-enrollments.component.ts
│   │   │   ├── my-enrollments.component.html
│   │   │   └── my-enrollments.component.css
│   │   │
│   │   └── students/                        # Directorio público institucional de alumnos
│   │       ├── students.component.ts
│   │       ├── students.component.html
│   │       └── students.component.css
│   │
│   ├── shared/                              # Componentes visuales transversales
│   │   └── navbar/
│   │       ├── navbar.component.ts
│   │       ├── navbar.component.html
│   │       └── navbar.component.css
│   │
│   ├── app.config.ts                        # Configuración global de la aplicación (Zoneless, Router, HTTP)
│   ├── app.routes.ts                        # Tabla de enrutamiento y asignación de Guardas
│   ├── app.ts                               # Componente raíz (Shell)
│   ├── app.html
│   └── app.css
│
├── environments/                            # Variables de entorno centralizadas
│   ├── environment.ts                       # Configuración de Producción
│   └── environment.development.ts           # Configuración de Desarrollo Local (:7210)
│
├── styles.css                               # Estilos globales y clases comunes centralizadas
└── main.ts                                  # Punto de entrada de bootstrapping
```

---

## 3. Desglose Técnico por Capas y Justificaciones

### 3.1. Arquitectura Zoneless y Gestión de Estado con Signals

En Angular convencional, el framework depende de `zone.js` para detectar cuándo un evento asíncrono (como un clic o una respuesta HTTP) ha terminado, ejecutando una verificación completa y descendente en todo el árbol de componentes (*Dirty Checking*).

#### ¿Por qué adoptamos Zoneless en esta solución?
1. **Rendimiento de Renderizado Quirúrgico:** Al usar `provideZonelessChangeDetection()`, Angular no intercepta las APIs del navegador. Los cambios son notificados exclusivamente por los **Signals** (`signal` y `computed`). Únicamente las partes del DOM que leen dicho signal son actualizadas.
2. **Cero Fugas de Memoria (*Memory Leaks*):** En lugar de gestionar complejas cadenas de `Subscription`, `takeUntilDestroyed` o `unsubscribe` manuales en cada componente, los Signals encapsulan valores sincrónicos reactivos.
3. **Cálculo Derivado en Tiempo Real:** En [`CatalogComponent`](file:///c:/Users/alvar/Documents/Prueba%20inter/src/frontend/src/app/features/catalog/catalog.component.ts), las reglas de negocio de créditos y profesores se calculan instantáneamente:
   ```typescript
   readonly currentCount = computed(() => this.academicService.myEnrollments()?.enrolledCoursesCount || 0);
   readonly currentCredits = computed(() => this.academicService.myEnrollments()?.totalCredits || 0);
   readonly isMaxReached = computed(() => this.currentCount() >= 3);
   
   readonly enrolledProfessorIds = computed(() =>
     this.academicService.myEnrollments()?.courses?.map(c => c.professorId) || []
   );
   ```
   Si el estudiante inscribe una materia, el `signal` de matrículas se actualiza y automáticamente:
   * El contador pasa de `0/9` a `3/9`.
   * Todas las materias del mismo docente se inhabilitan en la interfaz instantáneamente mediante `isConflict(course)`.

---

### 3.2. Segregación de Roles y Control de Acceso (RBAC)

El sistema distingue dos perfiles de usuario con naturalezas operativas completamente distintas:

| Perfil | Alcance Permitido | Vistas Prohibidas |
| :--- | :--- | :--- |
| **Estudiante** | Consultar catálogo, inscribir materias (hasta 3/9 créditos sin repetir docente), retirar materias, ver sus compañeros de curso y directorio institucional. | Panel Administrativo (`/admin`). |
| **Administrador** | Administrar docentes (crear, desactivar), administrar materias ofertadas (crear, desactivar) y consultar directorio de estudiantes. | Catálogo de inscripción (`/catalog`), Mis materias (`/my-enrollments`). |

#### Implementación Técnica:
1. **Guardas Funcionales (`core/guards/auth.guard.ts`):**
   * `studentGuard`: Si el usuario autenticado tiene rol de Administrador, el guarda intercepta la navegación y redirige automáticamente a `/admin`:
     ```typescript
     export const studentGuard: CanActivateFn = () => {
       const auth = inject(AuthService);
       const router = inject(Router);
       if (!auth.isAuthenticated()) return router.parseUrl('/login');
       if (auth.isAdmin()) return router.parseUrl('/admin');
       return true;
     };
     ```
   * `adminGuard`: Si un Estudiante intenta acceder a `/admin`, es desviado a `/catalog`.
   * `homeGuard`: En la ruta raíz (`/`), redirige de forma inteligente según el estado de sesión: Administrador a `/admin`, Estudiante a `/catalog`, o no autenticado a `/login`.
2. **Barra de Navegación Condicional ([`navbar.component.html`](file:///c:/Users/alvar/Documents/Prueba%20inter/src/frontend/src/app/shared/navbar/navbar.component.html)):**
   * El menú "Catálogo" y "Mis Materias" está blindado con `@if (authService.isStudent())`. El Administrador no tiene visibilidad de dichas opciones.

---

### 3.3. Integración con el Result Pattern del Backend

El backend no expone entidades directamente ni utiliza excepciones para control de flujo, sino que encapsula el 100% de las respuestas en `Result<T>`:

```json
{
  "isSuccess": true,
  "message": null,
  "errors": [],
  "result": { ... },
  "errorType": 0
}
```

#### Pipeline de Interceptores HTTP
1. **`authInterceptor`:** Lee el signal `authService.token()` y, si existe, clona la solicitud HTTP inyectando la cabecera `Authorization: Bearer {token}`.
2. **`apiEnvelopeInterceptor`:** Intercepta la respuesta exitosa `HttpResponse`. Si el cuerpo contiene la estructura `Result<T>`, **desempaqueta automáticamente la propiedad `result`**:
   ```typescript
   export const apiEnvelopeInterceptor: HttpInterceptorFn = (req, next) => {
     return next(req).pipe(
       map(event => {
         if (event instanceof HttpResponse) {
           const body = event.body as Result<unknown>;
           if (body && typeof body === 'object' && 'isSuccess' in body && 'result' in body) {
             return event.clone({ body: body.result });
           }
         }
         return event;
       })
     );
   };
   ```
   * **Justificación:** Los servicios frontend (`AcademicService`, `AuthService`) operan directamente con los tipos de dominio fuertemente tipados (`CourseDto[]`, `StudentEnrollmentSummaryDto`), abstrayendo por completo la envoltura de transporte.

---

### 3.4. Modularización de Modelos y Barrel Exports

Anteriormente concentrados en un único archivo monolítico, la capa [`core/models/`](file:///c:/Users/alvar/Documents/Prueba%20inter/src/frontend/src/app/core/models) se modularizó en 8 archivos especializados:

* Cada modelo reside en su propio archivo semántico (`course.model.ts`, `enrollment.model.ts`, etc.).
* El archivo [`index.ts`](file:///c:/Users/alvar/Documents/Prueba%20inter/src/frontend/src/app/core/models/index.ts) centraliza las exportaciones (*Barrel Pattern*), permitiendo importaciones limpias y estructuradas sin riesgo de dependencias circulares.

---

### 3.5. Centralización de Estilos y Diseño Visual

Para evitar duplicidad de reglas CSS en los componentes, se estableció una jerarquía clara:

1. **Estilos Globales y Compartidos ([`src/styles.css`](file:///c:/Users/alvar/Documents/Prueba%20inter/src/frontend/src/styles.css)):**
   * Tipografía y encabezados (`.page-title`, `.page-subtitle`).
   * Alertas de estado (`.alert-success`, `.alert-error`, `.privacy-alert`).
   * Controles de formulario (`.form-group`, labels, inputs y focus corporativo).
   * Botones de acción estándar (`.btn-submit`, `.btn-deactivate`, `.btn-drop`).
   * Contenedores de datos (`.table-card`, `.footer-links`).
2. **Estilos Específicos de Componente:**
   * Cada archivo `.component.css` retiene únicamente el layout propio de su vista (grids de materias, modales flotantes, insignias de estado).
   * **Beneficio:** Reducción drástica del tamaño total del bundle y coherencia estética en toda la suite.

---

### 3.6. Gestión de Variables de Entorno

Siguiendo el estándar de Angular 21, la configuración se desacopla entre entornos:

* **[`src/environments/environment.development.ts`](file:///c:/Users/alvar/Documents/Prueba%20inter/src/frontend/src/environments/environment.development.ts):** Apunta a `https://localhost:7210/api/v1` para depuración local.
* **[`src/environments/environment.ts`](file:///c:/Users/alvar/Documents/Prueba%20inter/src/frontend/src/environments/environment.ts):** Configuración lista para inyección en despliegues productivos.
* **Sustitución en `angular.json`:** El build de desarrollo utiliza `fileReplacements` automático, eliminando URLs quemadas en el código fuente.

---

## 4. Resumen de Decisiones de Arquitectura Frontend

| Decisión de Diseño | Implementación | Justificación y Beneficio Directo |
| :--- | :--- | :--- |
| **Detección de Cambios** | Zoneless (`provideZonelessChangeDetection`) | Elimina `zone.js`, renderizado ultra rápido guiado por Signals y menor tamaño de bundle. |
| **Reactividad** | Signals (`signal`, `computed`) | Reactividad atómica, derivaciones sincrónicas inmediatas y cero fugas de memoria. |
| **Arquitectura de Componentes** | Standalone Components | Desacoplamiento total, eliminación de `NgModules` y modularidad moderna de Angular. |
| **Separación de Archivos** | Tríada estricta (`.html`, `.css`, `.ts`) | Cumplimiento del estándar Angular, legibilidad y facilidad de mantenimiento. |
| **Seguridad y Rutas** | Guardas Funcionales (`CanActivateFn`) | Segregación estricta de roles (RBAC): Administrador vs. Estudiante. |
| **Integración HTTP** | Interceptor de Desenvoltura (`apiEnvelopeInterceptor`) | Consumo transparente del patrón funcional `Result<T>` del backend. |
| **Organización de Modelos** | Archivos individuales + *Barrel Export* | Código modular, tipado estricto y sin dependencias circulares. |
| **Estilos CSS** | Centralización en `styles.css` | Principio DRY (Don't Repeat Yourself), consistencia visual y ligereza en CSS de componentes. |
| **Configuración** | Angular Environments (`environment.ts`) | Desacoplamiento de URLs de servicio según el entorno de compilación. |
