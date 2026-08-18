# ADR-0003 — Sin singletons: registro de jugadores y referencias explícitas
2026-08-18 · Estado: aceptada

## Problema

El original tenía `GameManager.Instance`, `StealManager.Instance` y `AudioManager.Instance`,
consultados desde dentro de bucles de gameplay. Consecuencias reales: `MatchEnded` copiado
como guardia en cinco `Update()` distintos, imposibilidad de abrir una escena de prueba sin
arrastrar el juego entero, y orden de inicialización frágil.

Además, `Player1`/`Player2` estaba cableado por todas partes: enum `PlayerID`, `scores[2]`,
`players[2]`, `player1Holder`/`player2Holder`. Añadir un jugador o un modo tocaba diez archivos.

## Opciones consideradas

1. **Mantener singletons.** Cómodo a corto plazo, es exactamente lo que nos trajo aquí.
2. **Framework de inyección de dependencias** (Zenject/VContainer). Resuelve el problema,
   pero es mucha maquinaria y curva de aprendizaje para un equipo de 3 y un juego pequeño.
3. **Referencias serializadas + un coordinador + un registro de jugadores.**

## Decisión

Opción 3. `MatchController` es un coordinador normal, referenciado explícitamente.
`PlayerRegistry` guarda los jugadores que se registraron al empezar la partida, con su
`PlayerIdentity` y su `PlayerProfile`. Ningún `X.Instance` en el proyecto.

## Razón

Elimina el problema real (dependencias invisibles y estado global) sin pagar el coste de un
contenedor de DI. Las dependencias quedan visibles en el inspector y en el constructor
lógico de cada componente, que es lo que un equipo de tres necesita para no pisarse.

## Consecuencias

- Los sistemas iteran jugadores registrados en vez de asumir dos.
- Los eventos identifican a los jugadores por `int` id, no por enum.
- Hay que arrastrar referencias en prefabs y escenas: si algo queda sin asignar, se ve.
- Si algún día hace falta un tercer jugador o un modo distinto, no es un refactor global.
