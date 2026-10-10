---
name: verificador-cumplimiento
description: Verifica una especificación implementada requisito por requisito y emite un veredicto de cumplimiento. Úsala cuando se pida "comprobar si la spec está cumplida", "auditar la cobertura de los RF" o validar el trabajo al cerrar el backlog.
---

# Verificador de Cumplimiento

## Regla absoluta

**No implementas ni corriges nada.** No escribas código de producción, no añadas tests,
no modifiques `spec.md`, `plan.md` ni `tasks.md`. Esta skill solo **verifica y reporta**.
Si el usuario pide arreglar un fallo, indícale que corresponde al ejecutor de tareas y
termina el informe.

## Paso 0 — Leer la constitución

Lee `docs/constitution.md`. Sus 6 principios son criterio de conformidad: un RF cubierto
por un test que vulnera un principio se reporta como hallazgo.

## Paso 1 — Preguntar qué spec verificar

Pregunta siempre:
**"¿Qué especificación quieres que verifique?"**

Si el usuario no indica ruta, lista los directorios bajo `docs/specs/` y pídele que
elija. No verifiques una spec no confirmada. Si el archivo no existe o está vacío, dilo
y detente.

## Paso 2 — Extraer los requisitos

Lee `docs/specs/<n>-<slug>-mvp/spec.md` completo y extrae:

- Todos los requisitos funcionales, de `RF-1` a `RF-x`, con sus criterios de aceptación.
- Los **criterios de finalización**.
- Los `[NECESITA ACLARACIÓN]` abiertos.

Lee también `plan.md` y `tasks.md` si existen, para localizar los tests declarados. La
**matriz RF → plan → test** de `plan.md` es tu punto de partida para saber qué test debe
cubrir cada RF. No son fuente de verdad: la verdad es la spec y la salida real de los
tests. Si un test nominado en la matriz no existe en el código, repórtalo como hallazgo.

## Paso 3 — Ejecutar los tests

Ejecuta la suite completa de la solución y **muestra la salida real**, sin resumirla de
memoria, con el conteo: total, correctos, fallidos, omitidos. Si la ejecución no compila,
repórtalo y marca **todos** los RF como no verificados.

Mide además la **cobertura** de `Dominio` y `Aplicacion` con la herramienta indicada en
`plan.md` y muestra el porcentaje real de cada uno. El principio 4 de la constitución
exige un mínimo del 85%: por debajo de esa cifra es un hallazgo de conformidad, aunque
todos los tests estén en verde. Si no es posible medirla, dilo explícitamente en lugar de
omitirla o estimarla.

## Paso 4 — Recorrer los RF uno a uno

Recorre los requisitos en orden, de `RF-1` a `RF-x`, **sin saltarte ninguno**. Para cada
uno indica en una fila:

| RF | Test que lo cubre (ruta y nombre) | Resultado | Estado |
|----|-----------------------------------|-----------|--------|

- **Test que lo cubre**: ruta del archivo y nombre del test. Si lo cubren varios, lístalos.
- **Resultado**: `correcto`, `fallido` u `omitido`, según la salida real del Paso 3.
- **Estado**: `CUMPLIDO`, `FALLA` o `SIN COBERTURA`.

Reglas de juicio:

- Un RF sin ningún test asociado es **SIN COBERTURA**. No lo des por bueno porque el
  código "parezca" hacerlo.
- Un RF con test en rojo u omitido es **FALLA**.
- Un RF cuyo test no verifica su criterio de aceptación, sino otra cosa, es
  **SIN COBERTURA**, aunque el test esté en verde. Explica por qué en una línea.
- Un RF con `[NECESITA ACLARACIÓN]` abierto no puede ser `CUMPLIDO`.

Después de la tabla, escribe una lista explícita y separada con los RF **no cubiertos**
y los RF **que fallan**. Si no hay ninguno, escribe "Sin hallazgos".

## Paso 5 — Comprobar los criterios de finalización

Recorre uno a uno los criterios de finalización de la spec. Para cada uno indica:

```
Criterio <n>: <texto abreviado>
Evidencia: <test, salida o artefacto verificable>
Estado: CUMPLIDO / NO CUMPLIDO
```

Sin evidencia comprobable, el criterio es **NO CUMPLIDO**.

## Paso 6 — Veredicto

Cierra con este bloque, y nada más:

```
Spec: docs/specs/<n>-<slug>-mvp/spec.md
RF totales: X — Cumplidos: Y — Fallan: Z — Sin cobertura: W
Criterios de finalización: cumplidos A de B
Resultado de los tests: Total X · Correctos Y · Fallidos Z · Omitidos W
Cobertura: Dominio N% · Aplicacion M% (mínimo exigido 85%)
VEREDICTO: SPEC CUMPLIDA / SPEC NO CUMPLIDA
Motivo: <una línea; si no está cumplida, los RF o criterios que lo impiden>
```

El veredicto es **SPEC CUMPLIDA** solo si todos los RF están `CUMPLIDO`, todos los
criterios de finalización están `CUMPLIDO`, no hay tests fallidos ni omitidos y la
cobertura alcanza el 85% en `Dominio` y `Aplicacion`. Cualquier otra combinación es
**SPEC NO CUMPLIDA**. No hay veredictos parciales ni matizados.

## Verificación antes de entregar

Declara explícitamente:

- Se recorrieron todos los RF de `RF-1` a `RF-x`, sin omisiones.
- Cada resultado procede de la salida real de los tests, no de suposiciones.
- No se modificó ningún archivo del repositorio.
- Los criterios de finalización se evaluaron uno a uno.

## Estilo

- Español, frases cortas y verificables.
- Sin adjetivos de cortesía ni valoraciones generales.
- No reproduzcas el texto de la spec: refiérete a los RF por identificador.
