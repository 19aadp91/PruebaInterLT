# Especificación Funcional: Plataforma de Registro de Estudiantes e Inscripción de Materias (MVP)

## 1. Contexto y objetivo
El sistema permite gestionar el ciclo de vida de registro de estudiantes y su posterior inscripción académica en un programa basado en créditos. El objetivo es garantizar una asignación académica equitativa y variada, aplicando de manera estricta la regla de negocio que prohíbe cursar múltiples materias con el mismo docente, preservando además la privacidad de los datos personales entre los alumnos.

---

## 2. Usuarios
* **Estudiante:** Usuario que se registra en la plataforma, gestiona su perfil e inscribe, modifica o retira asignaturas dentro de su tope de créditos, y consulta los compañeros de sus clases.
* **Administrador:** Usuario responsable de gestionar el catálogo académico de profesores, asignaturas y la vinculación docente-materia.

---

## 3. Historias de usuario
* **HU-1:** Como estudiante, quiero registrar mi cuenta en línea con mis datos básicos, para adherirme al programa académico de créditos.
* **HU-2:** Como estudiante, quiero autenticarme en la plataforma mediante credenciales seguras, para acceder a mis servicios académicos de forma privada.
* **HU-3:** Como estudiante, quiero consultar la oferta de asignaturas y sus profesores asignados, para planificar mi carga académica.
* **HU-4:** Como estudiante, quiero inscribir entre una y tres asignaturas respetando la no repetición de profesores, para avanzar en mi plan de estudios.
* **HU-5:** Como estudiante, quiero modificar o retirar asignaturas previamente inscritas, para ajustar mi horario antes del cierre de inscripciones.
* **HU-6:** Como estudiante, quiero ver el listado de otros estudiantes registrados, para conocer la comunidad académica viendo únicamente sus nombres.
* **HU-7:** Como estudiante, quiero consultar la lista de compañeros de cada una de mis clases, para coordinar actividades conociendo exclusivamente sus nombres.
* **HU-8:** Como administrador, quiero gestionar los profesores y las asignaturas disponibles, para mantener actualizada la oferta educativa del programa.

---

## 4. Requisitos funcionales

### RF-1: Registro de Estudiantes en Línea
* El sistema debe permitir el autoregistro de estudiantes solicitando nombre completo, documento de identificación, correo electrónico y contraseña.
* Si el documento de identificación o el correo electrónico ya existen en el sistema, entonces el sistema debe rechazar el registro indicando la duplicidad.
* Si la contraseña proporcionada cuenta con menos de 6 caracteres, entonces el sistema debe rechazar el registro indicando la longitud mínima requerida.
* Si alguno de los campos obligatorios se encuentra vacío o con formato inválido, entonces el sistema debe rechazar el registro con un mensaje explicativo.

### RF-2: Autenticación y Control de Acceso
* Cuando el usuario ingresa sus credenciales válidas, el sistema debe autenticarlo y autorizar el acceso según su rol asignado.
* Si las credenciales son incorrectas, entonces el sistema debe denegar el acceso sin revelar cuál de los datos fue erróneo.
* Mientras la sesión esté activa, el sistema debe restringir las operaciones administrativas exclusivamente a los usuarios con rol de administrador.

### RF-3: Consulta del Catálogo de Materias y Profesores
* El sistema debe exponer el catálogo de asignaturas disponibles indicando nombre, código, créditos (3 créditos por materia) y el profesor asignado.
* Mientras el estudiante explora la oferta, el sistema debe señalar la disponibilidad de cada materia y el docente a cargo.

### RF-4: Inscripción y Selección de Materias
* Cuando el estudiante selecciona una asignatura para inscribir, el sistema debe validar que no supere el límite máximo de tres materias (9 créditos).
* Cuando el estudiante intenta agregar una materia cuyo profesor ya imparte otra materia actualmente inscrita por el estudiante, entonces el sistema debe bloquear la selección indicando la incompatibilidad de docente.
* Cuando el estudiante intenta inscribir una materia que ya tiene inscrita, entonces el sistema debe impedir la selección por duplicidad.
* El sistema debe permitir guardar y confirmar una inscripción parcial de una, dos o tres asignaturas.
* Si el estudiante intenta confirmar una inscripción con más de tres asignaturas, entonces el sistema debe rechazar la operación.

### RF-5: Modificación y Retiro de Materias Inscritas
* Cuando el estudiante solicita retirar una asignatura inscrita, el sistema debe desvincularla inmediatamente y liberar el crédito y el cupo del docente asociado.
* Cuando el estudiante solicita reemplazar una materia inscrita por otra, el sistema debe revaluar las reglas de no repetición de docente y el límite de materias antes de consolidar el cambio.

### RF-6: Consulta Pública de Estudiantes Registrados
* Cuando un estudiante autenticado consulta el registro general de estudiantes, el sistema debe listar a los estudiantes mostrando única y exclusivamente su nombre completo.
* El sistema debe ocultar y proteger en esta consulta el documento de identificación, correo electrónico y cualquier dato sensible de los demás estudiantes.

### RF-7: Consulta de Compañeros de Clase
* Cuando un estudiante consulta el detalle de una clase en la que está inscrito, el sistema debe listar los compañeros que comparten esa misma materia.
* El sistema debe mostrar exclusivamente el nombre completo de los compañeros de clase, omitiendo identificadores, correos o datos de contacto.

### RF-8: Gestión Administrativa de Profesores y Asignaturas
* El sistema debe permitir al administrador crear, modificar y deshabilitar registros de profesores.
* El sistema debe permitir al administrador crear, modificar y deshabilitar asignaturas asignándoles un profesor responsable.
* Si una asignatura tiene estudiantes inscritos activos, entonces el sistema debe impedir su eliminación física para preservar la consistencia académica.
* Si un profesor tiene asignaturas asociadas con inscripciones activas, entonces el sistema debe impedir su desvinculación hasta que las asignaturas queden liberadas.

---

## 5. Requisitos no funcionales
* **Seguridad:** Todas las contraseñas deben almacenarse mediante funciones criptográficas de derivación de claves; las comunicaciones deben realizarse bajo canales cifrados y las sesiones deben expirar de forma controlada.
* **Privacidad de Datos:** El sistema debe aplicar aislamiento estricto de datos personales en consultas compartidas, exponiendo exclusivamente el nombre completo a terceros.
* **Trazabilidad:** Cada inscripción, modificación o cancelación de asignatura debe registrar fecha y hora exacta en formato UTC.
* **Usabilidad:** La interfaz de selección de asignaturas debe advertir visualmente e inhabilitar las materias cuyo profesor ya se encuentre seleccionado.
* **Integridad:** Las transacciones de inscripción deben ser atómicas para evitar inconsistencias en selecciones concurrentes.

---

## 6. Casos límite
* **Intento de selección de dos materias del mismo profesor simultáneamente:** El sistema debe deshabilitar la segunda materia en cuanto se seleccione la primera.
* **Intento de sobrepasar los 9 créditos:** Al tener 3 materias seleccionadas, la interfaz y el servicio deben bloquear cualquier selección adicional.
* **Inscripción de 0 materias:** Guardar una inscripción con cero materias debe considerarse equivalente a no tener materias registradas o retiro total.
* **Concurrencia en retiro y reemplazo:** Si un estudiante retira una materia y agrega otra en la misma solicitud, la regla de docente se evalúa contra el conjunto resultante.
* **Consulta de clase sin compañeros:** Cuando solo un estudiante esté matriculado en una materia, la lista de compañeros debe mostrarse vacía sin error.

---

## 7. Fuera de alcance
* Control de cupos máximos de estudiantes por aula/materia (cupos ilimitados para este MVP).
* Gestión de días, franjas horarias y cruces de horario de clases.
* Calificaciones, notas académicas y asistencias.
* Pasarelas de pago o cobro de créditos académicos.

---

## 8. Criterios de finalización
1. Registro de estudiantes y autenticación con emisión de credenciales funcionales.
2. Catálogo inicial precargado con 5 profesores y 10 materias (2 materias por profesor).
3. Inscripción exitosa de hasta 3 materias (9 créditos) por estudiante.
4. Bloqueo total y comprobado de inscripción de 2 materias con el mismo docente para un mismo estudiante.
5. Listado de estudiantes registrados donde solo sea visible el nombre completo.
6. Consulta de compañeros por clase donde solo sea visible el nombre completo de los demás alumnos.
7. Módulo administrativo para gestión de docentes y materias operativo.

---

## 9. Dudas abiertas
* Sin dudas abiertas. Todos los requisitos y casos frontera han sido acordados.
