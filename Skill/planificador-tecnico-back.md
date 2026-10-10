---
name: planificador-tecnico
description: Redacta el plan técnico a partir de una especificación aprobada. Úsala cuando se pida "crear el plan", "planificar la implementación" o definir arquitectura y estrategia de tests antes de codificar.
---

# Planificador Técnico

## Regla absoluta

**NO escribas código en ningún momento.** Ni fragmentos, ni firmas de métodos, ni
esquemas de clases, ni consultas, ni contratos JSON. El plan describe el **CÓMO** en
prosa y tablas; la implementación es una fase posterior.

## Paso 0 — Leer la constitución

Lee `docs/constitution.md` antes de cualquier decisión. Sus 6 principios son
restricciones duras: stack permitido, separación de capas, política de tests,
persistencia e idioma. Ninguna decisión del plan puede contradecirlos.

## Paso 1 — Preguntar qué spec planificar

Pregunta siempre:
**"¿Para qué especificación quieres que genere el plan?"**

Si el usuario no indica una ruta concreta, lista los directorios existentes bajo
`docs/specs/` y pídele que elija. No planifiques sobre una spec no confirmada. Si el
archivo no existe o está vacío, dilo y detente.

## Paso 2 — Leer la spec completa

Extrae el listado íntegro de requisitos funcionales (`RF-1`, `RF-2`, ...), los no
funcionales, los casos límite y lo declarado fuera de alcance. Si la spec tiene
`[NECESITA ACLARACIÓN]` abiertos, señálalos y planifica solo lo que está cerrado,
dejando registrado el bloqueo.

## Paso 3 — Preguntas técnicas

Antes de decidir nada, consulta al usuario las opciones técnicas que el plan debe fijar.

Reglas:

- Preguntas **de UNA en UNA**, esperando respuesta antes de la siguiente.
- **Máximo 6 preguntas.** Solo sobre decisiones que cambian el plan; si la constitución ya
  la resuelve, no preguntes.
- En cada pregunta **sugiere 2 o 3 opciones concretas** y marca la que recomiendas, con su
  motivo en una línea.
- Si el usuario responde "lo que veas", adopta tu recomendación y déjala registrada como
  decisión técnica.
- Si una respuesta contradice la constitución, dilo, cita el principio y vuelve a
  preguntar. Esa repregunta no consume cupo.
- Ninguna respuesta puede introducir código: se responde eligiendo opciones, no
  escribiendo implementación.

Temas a cubrir, por orden de impacto:

1. **Patrones por caso concreto**
   - Orquestación de casos de uso: mediador ya existente en la solución, invocación
     directa de servicios de aplicación.
   - Acceso a datos: repositorio por agregado, repositorio genérico existente, unidad de
     trabajo.
   - Integración con servicios externos: adaptador con puerto en `Aplicacion`, cliente
     tipado, fachada.
   - Resiliencia: reintentos, cortocircuito, sin política.
   - Resultado de operaciones: excepciones de dominio, objeto resultado, códigos de error.
   - Creación de objetos complejos: fábrica, constructor, mapeo directo.
2. **Paquetes NuGet**
   - Recuerda el principio 1: solo las dependencias actuales; cualquier paquete nuevo
     exige justificación escrita.
   - Pregunta expresamente si se admite algún paquete nuevo para validación, mapeo,
     pruebas o dobles de prueba, y recoge su justificación.
   - Si la respuesta es no, planifica con lo ya presente en la solución.
3. **Estrategia de pruebas**
   - Marco de pruebas y librería de dobles.
   - Alcance de las pruebas de integración: contra servicios reales, contra sustitutos o
     ninguna.
4. **Contratos expuestos y de consumo**
   - Versionado de la API, formato de error, convención de nombres en la respuesta.
5. **Configuración y despliegue**
   - Origen de parámetros, zonas horarias, tratamiento de secretos.
6. **Rendimiento y concurrencia**
   - Ejecución secuencial o en paralelo de llamadas externas, límites de concurrencia.

Cierra el paso con un **resumen numerado de lo acordado**. Cada respuesta se traslada al
bloque "Decisiones técnicas" del plan con su alternativa descartada.

## Paso 4 — Generar el plan

Crea `docs/specs/<n>-<slug>-mvp/plan.md`, en el **mismo directorio de la spec** que se
está planificando (misma `<n>` y mismo `<slug>`). Estructura obligatoria:

1. **Resumen** — qué se va a construir y sobre qué spec se basa.
2. **Estructura de módulos** — qué se añade o modifica en cada capa (`Dominio`,
   `Aplicacion`, `Infraestructura`, `Api`), responsabilidad de cada pieza y sentido de
   las dependencias. Cada elemento indica los **RF que cubre**.
3. **Decisiones técnicas** — una entrada por decisión, con:
   - Decisión adoptada.
   - Justificación, ligada al principio de la constitución que la respalda.
   - **Alternativa descartada** y motivo del descarte.
   - RF afectados.
4. **Estrategia de tests** — qué se prueba con tests unitarios en `Dominio` y
   `Aplicacion`, qué requiere pruebas de integración, cómo se cubren los casos límite de
   la spec y cómo se alcanza la cobertura mínima exigida por la constitución.
5. **Matriz de cobertura RF → plan → test** — tabla con una fila por cada RF de la spec
   y estas columnas: RF, módulo responsable, **nombre previsto del test** y tipo de
   prueba (unitaria o de integración). El nombre del test es obligatorio: es la entrada
   que consumirá la fase de verificación. No vale "por definir".
6. **Riesgos y bloqueos** — incluidos los `[NECESITA ACLARACIÓN]` heredados.

## Paso 5 — Verificación antes de entregar

Comprueba y declara explícitamente al final del plan:

- **Cobertura total**: cada RF de la spec aparece al menos una vez en la matriz. Si
  alguno queda sin cubrir, márcalo como defecto del plan, no lo omitas.
- **Test nominado**: cada fila de la matriz indica un nombre de test concreto. Una fila
  sin test nominado es un defecto del plan.
- **Cobertura medible**: la estrategia de tests indica cómo se mide el 85% exigido por el
  principio 4 y con qué herramienta ya disponible en la solución.
- **Conformidad**: ninguna decisión introduce dependencias, capas ni almacenes prohibidos
  por la constitución. Cita el principio verificado.
- **Alcance**: el plan no incorpora nada declarado fuera de alcance en la spec.
- **Trazabilidad de las respuestas**: cada acuerdo del Paso 3 aparece como decisión
  técnica, con su alternativa descartada. Si algún paquete nuevo fue aceptado, su
  justificación escrita queda en el plan, conforme al principio 1.

## Estilo

- Español, frases cortas y verificables.
- Toda decisión técnica lleva justificación; nada de "porque es mejor".
- No repitas el texto de la spec: refiérete a los RF por su identificador.

