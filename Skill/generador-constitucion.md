---
name: generador-constitucion
description: Redacta la constitución de un proyecto nuevo con 6 principios innegociables. Úsala cuando se pida "crear la constitución", "definir los principios del proyecto" o antes de generar la primera especificación.
---

# Generador de Constitución

## Regla absoluta

**NO escribas código en ningún momento.** Ni fragmentos, ni firmas, ni esquemas de
clases, ni archivos de configuración. La constitución fija principios, no
implementación. Tampoco redactes specs, planes ni tareas: son fases posteriores.

## Paso 1 — Preguntar la idea del proyecto

Pregunta siempre, aunque creas saber la respuesta:
**"¿Cuál es la idea del proyecto y con qué tecnología quieres construirlo?"**

Necesitas ambas cosas antes de continuar:

1. **Idea** — qué problema resuelve el proyecto y para quién.
2. **Tecnología** — lenguaje, plataforma y, si existe, marco de trabajo elegido.

Si el usuario responde solo una de las dos, pregunta por la que falta. No inventes ni
asumas stack, dominio ni alcance. Si responde "lo que veas", propón una opción concreta
con su motivo en una línea y pide confirmación.

## Paso 2 — Preguntas de desambiguación

Solo si algo impide redactar un principio verificable. **Máximo 3 preguntas**, de UNA en
UNA, esperando respuesta antes de la siguiente. Prioriza:

- Motor de datos o ausencia de persistencia.
- Existencia de interfaz de usuario y su separación de la lógica.
- Idioma exigido en código, mensajes y documentación.

Si una respuesta queda incompleta, no gastes otra pregunta: márcala como
`[NECESITA ACLARACIÓN]` dentro del principio afectado.

## Paso 3 — Proponer la constitución

Propón el contenido de `docs/constitution.md` con **exactamente 6 principios
innegociables**, numerados y en este orden:

1. **Simplicidad del stack** — tecnologías y dependencias permitidas; qué exige una
   dependencia nueva.
2. **Relación entre spec y código** — todo código traza a un RF de una spec aprobada.
3. **Separación entre lógica e interfaz** — qué capa no puede depender de qué.
4. **Política de tests** — qué debe tener test obligatorio y cuándo se escribe.
5. **Persistencia de datos** — dónde y cómo se accede al almacén.
6. **Idioma del código y los mensajes** — alcance del idioma acordado.

Restricciones de forma:

- **Máximo 15 líneas** en total el documento propuesto, incluido el título.
- Un principio por línea, en frase corta y **verificable**: debe poder decirse "se
  cumple" o "no se cumple" sin interpretar.
- Prohibidos "limpio", "moderno", "robusto", "buenas prácticas" y demás términos sin
  criterio de comprobación.
- Los principios reflejan la tecnología e idea confirmadas en el Paso 1; nada genérico.

## Paso 4 — Esperar aprobación

Muestra la propuesta y pregunta:
**"¿Apruebas esta constitución o quieres cambios?"**

**No crees ni modifiques `docs/constitution.md` sin aprobación explícita.** Si el usuario
pide cambios, aplícalos y vuelve a pedir aprobación, respetando siempre el límite de 6
principios y 15 líneas. Solo con un "sí" claro escribe el archivo en `docs/constitution.md`.

## Verificación antes de entregar

Declara explícitamente:

- Hay 6 principios, ni uno más ni uno menos.
- El documento no supera las 15 líneas.
- Cada uno de los 6 temas exigidos está cubierto por su principio.
- Cada principio es comprobable de forma objetiva.
- No se escribió el archivo antes de la aprobación del usuario.

## Estilo

- Español, frases cortas y directas.
- Redacción en presente y en tono normativo: "debe", "no puede", "solo".
- Sin justificaciones largas ni ejemplos: la constitución se cita, no se explica.
