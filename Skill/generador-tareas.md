---
name: generador-tareas
description: Descompone una especificación y su plan en tareas ejecutables. Úsala cuando se pida "generar las tareas", "desglosar el trabajo" o preparar el backlog antes de implementar.
---

# Generador de Tareas

## Regla absoluta

**NO escribas código en ningún momento.** Las tareas describen trabajo a realizar, no su
implementación. Nada de fragmentos, firmas, esquemas ni consultas.

## Paso 0 — Leer la constitución

Lee `docs/constitution.md`. Las tareas deben respetar sus 6 principios, en especial la
separación de capas, la política de tests y el idioma.

## Paso 1 — Preguntar qué funcionalidad desglosar

Pregunta siempre:
**"¿De qué especificación quieres que genere las tareas?"**

Si el usuario no indica una ruta, lista los directorios bajo `docs/specs/` y pídele que
elija. No continúes sin confirmación.

## Paso 2 — Leer spec y plan

Lee **ambos** archivos del directorio elegido: `spec.md` y `plan.md`. Si falta alguno,
dilo y detente: no se generan tareas sin plan aprobado. Extrae todos los RF de la spec y
la estructura de módulos, decisiones y estrategia de tests del plan.

## Paso 3 — Generar las tareas

Crea `docs/specs/<n>-<slug>-mvp/tasks.md`, en el mismo directorio de la spec y el plan.

Reglas de descomposición:

- Cada tarea debe completarse en **20-30 minutos como máximo**. Si excede ese tamaño,
  divídela.
- Las tareas van en **orden de dependencia**: nada depende de una tarea posterior.
- Sigue el flujo de capas del plan: primero `Dominio`, luego `Aplicacion`, después
  `Infraestructura` y por último `Api`.
- Los tests de una regla de negocio son tarea propia y van junto a la regla que verifican.
- Una tarea, un resultado observable. Nada de tareas comodín tipo "ajustes varios".

Formato obligatorio de cada tarea:

```
- [ ] **T-01** — <acción concreta>
  - Cubre: RF-1, RF-3
  - Depende de: T-00 (o "ninguna")
  - Hecho cuando: <condición verificable>
```

La línea **"Hecho cuando:"** debe ser comprobable por un tercero sin interpretar: un test
que pasa, un endpoint que responde algo concreto, un dato persistido. Prohibido
"funciona correctamente" o "está implementado".

Estructura del documento:

1. **Resumen** — spec y plan de origen, número total de tareas.
2. **Tareas** — lista de checkboxes en orden de dependencia, agrupadas por capa.
3. **Matriz RF ? tareas** — una fila por RF con las tareas que lo cubren.

## Paso 4 — Verificación antes de entregar

Declara explícitamente al final:

- Todo RF de la spec tiene al menos una tarea asociada; si alguno no la tiene, márcalo
  como hueco, no lo omitas.
- No hay tareas sin RF asociado salvo las de soporte, que deben justificarse.
- El orden no contiene dependencias circulares ni hacia adelante.
- Cada regla de cálculo tiene su tarea de test, conforme al principio 4.

## Estilo

- Español, frases cortas y en infinitivo o imperativo.
- Identificadores correlativos `T-01`, `T-02`, ...
- No repitas el texto de la spec ni del plan: refiérete a los RF por identificador.
