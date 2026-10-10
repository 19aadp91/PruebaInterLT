---
name: generador-especificaciones
description: Redacta especificaciones funcionales de una idea antes de escribir código. Úsala cuando se pida "crear una spec", "especificar una funcionalidad" o antes de iniciar cualquier nuevo desarrollo.
---

# Generador de Especificaciones

## Regla absoluta

**NO escribas código en ningún momento.** Ni fragmentos, ni firmas, ni pseudocódigo, ni
esquemas de clases, ni contratos JSON. Si el usuario pide código, recuérdale que esta
skill solo produce la especificación y que el código pertenece a la fase de plan.

## Paso 0 — Leer la constitución

Antes de cualquier otra acción, lee `docs/constitution.md` y respeta sus 6 principios.
Son innegociables y la especificación no puede contradecirlos.

## Paso 1 — Preguntar la idea

Si el usuario no ha indicado explícitamente la funcionalidad, pregúntale primero:
**"¿Cuál es la idea o funcionalidad para la que quieres la especificación?"**
No asumas ni inventes la funcionalidad.

## Paso 2 — Preguntas de desambiguación

Haz preguntas **de UNA en UNA**, esperando la respuesta antes de la siguiente.
**Máximo 6 preguntas.** Cada pregunta debe eliminar una ambigüedad real, priorizando:

- Casos límite y valores frontera.
- Comportamiento ante errores y entradas inválidas.
- Qué queda explícitamente **fuera** del MVP.
- Reglas de negocio con más de una interpretación posible.

No hagas preguntas sobre stack, librerías, base de datos ni estructura de archivos.
Si una respuesta queda incompleta, no gastes otra pregunta: márcala como
`[NECESITA ACLARACIÓN]` en la spec.

## Paso 3 — Generar la especificación

Con las respuestas, crea el archivo `docs/specs/<n>-<slug>-mvp/spec.md`, donde `<n>` es
el siguiente número incremental de tres dígitos según los directorios ya existentes en
`docs/specs/` (principio 2 de la constitución).

Reglas de numeración:

- Considera solo los directorios que ya cumplen el patrón `<n>-<slug>-mvp`. Si encuentras
  alguno que no lo cumple, señálalo y no lo uses para calcular `<n>`.
- `<n>` es el mayor número existente más uno, con tres dígitos (`001`, `002`, ...). No
  reutilices huecos de specs eliminadas.
- **Si el directorio calculado ya existe, deténte y pregúnta al usuario.** Nunca
  sobrescribas ni fusiones una spec existente.
- Las rutas son relativas a la raíz del repositorio, no a `docs/`.

Estructura obligatoria del documento, en este orden:

1. **Contexto y objetivo** — el problema y por qué importa.
2. **Usuarios** — perfiles afectados y qué buscan.
3. **Historias de usuario** — formato "Como \<rol\>, quiero \<acción\>, para \<beneficio\>".
4. **Requisitos funcionales** — numerados `RF-1`, `RF-2`, ..., cada uno con sus
   criterios de aceptación en **notación EARS en español**:
   - Ubicuo: *El sistema debe \<respuesta\>.*
   - Dirigido por evento: *Cuando \<disparador\>, el sistema debe \<respuesta\>.*
   - Por estado: *Mientras \<estado\>, el sistema debe \<respuesta\>.*
   - Condicional opcional: *Donde \<característica\>, el sistema debe \<respuesta\>.*
   - No deseado: *Si \<condición no deseada\>, entonces el sistema debe \<respuesta\>.*
5. **Requisitos no funcionales** — rendimiento, seguridad, trazabilidad, usabilidad.
6. **Casos límite** — lista explícita de escenarios frontera y de error.
7. **Fuera de alcance** — lo que el MVP no hará.
8. **Criterios de finalización** — condiciones verificables para dar por hecha la funcionalidad.
9. **Dudas abiertas** — cada una marcada como `[NECESITA ACLARACIÓN]`.

## Paso 4 — Alcance del contenido

La spec describe el **QUÉ** y el **POR QUÉ**. Está prohibido incluir:

- Nombres de tecnologías, frameworks, paquetes o motores de datos.
- Arquitectura, capas, patrones o diagramas de componentes.
- Nombres de archivos, clases, métodos, tablas, colecciones o endpoints.
- Estimaciones de esfuerzo o decisiones de implementación.

Todo eso pertenece al plan, en una fase posterior.

## Estilo

- Redacta en español, en frases cortas y verificables.
- Cada requisito debe ser comprobable: evita "rápido", "amigable", "robusto" sin métrica.
- Un requisito por línea; nada de requisitos compuestos con "y" encubierto.
