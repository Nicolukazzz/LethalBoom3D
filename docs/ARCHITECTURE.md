# Architecture

Regla que ordena todo lo demás: **el gameplay emite eventos; la presentación se suscribe.**

## Ensamblados

```
Game.Core  ──►  Game.Input  ──►  Game.Gameplay  ──►  Game.Presentation
                                                     Game.Editor (solo editor)
```

Las flechas son la dirección permitida de referencias. `Game.Gameplay` **no puede**
referenciar `Game.Presentation`: el compilador impide que se cuele un `ParticleSystem`
dentro de la lógica.

Cuatro ensamblados, no uno por carpeta: con cinco sistemas de gameplay que se hablan entre
sí, un `.asmdef` por sistema añade fricción sin proteger nada. Se dividirá `Game.Gameplay`
solo si aparece una razón concreta.

## Comunicación

| Distancia | Mecanismo |
|---|---|
| Componentes del mismo objeto/prefab | referencia serializada directa |
| Componentes vecinos (jugador ↔ disco) | evento C# del componente concreto |
| Entre sistemas (gameplay → HUD/audio/VFX/métricas) | `GameplayEventChannel` (ScriptableObject) |

`GameplayEventChannel` vive en `Assets/Game/Config/GameplayEventChannel.asset` y se arrastra
a quien lo necesite. Al ser un asset y no un singleton, una escena sandbox puede usar su
propio canal sin arrastrar el resto del juego.

Los eventos son structs `readonly` en `Core/GameplayEvents.cs`. Añadir una mecánica
relevante para el balance = añadir su evento. Esa lista es también la de métricas de
playtest (ver `PLAYTESTING.md`).

## Sistemas

### Core
Eventos, canal de eventos, constantes de layers. Sin dependencias.

### Input (`Game.Input`)
`PlayerInputRouter` es el **único** componente que toca el Input System. Expone
`MoveInput`, `AimDirection` y eventos (`JumpPressed`, `AimThrowPressed`,
`PlacePortalPressed`, `ReadyPressed`, `PausePressed`). El gameplay no sabe si la acción
vino de `W`, del stick o de `R2`.

Un `PlayerInput` por jugador con esquemas `Keyboard Left`, `Keyboard Right` y `Gamepad`;
el emparejamiento de dispositivos lo hace el Input System, así que soportar gamepads es
configuración, no código.

### Player (`Gameplay/Player`)
Componentes pequeños sobre un mismo prefab, sin que ninguno sepa "soy el jugador 1":

| Componente | Responsabilidad |
|---|---|
| `PlayerIdentity` | id de runtime + `PlayerProfile` (nombre, color) |
| `PlanarMovement` | correr, saltar, gravedad; `Rigidbody` con `FreezePositionZ` |
| `GroundCheck` | detección de suelo por `SphereCast` contra `groundMask` |
| `PlayerAbilities` | modificadores acumulables (velocidad, gravedad, bloqueo de salto) |
| `DiscCarrier` | tengo/no tengo disco + punto de mano |
| `Aiming` | estado de apuntado; delega la dirección a un `IAimResolver` |
| `Stunnable` | muerte/aturdimiento y respawn, **sin** `SetActive(false)` |

`PlayerAbilities` existe porque en el original el componente de disco escribía
directamente propiedades del de movimiento; con modificadores, dos sistemas pueden
ralentizar al jugador sin pisarse.

### Disc (`Gameplay/Disc`)

| Componente | Responsabilidad |
|---|---|
| `Disc` | fachada mínima y estado (`Held`, `InFlight`, `Loose`) |
| `DiscPossession` | quién lo tiene; eventos de cambio de posesión |
| `DiscFlight` | lanzamiento, rebotes, límite de velocidad |
| `BombTimer` | cuenta solo en posesión; evento al explotar |
| `DiscPickup` | recogida/intercepción por `OverlapSphere` con inmunidad del lanzador |

`DiscPossession` no conoce jugadores concretos, solo `DiscCarrier`s. En el original el
disco guardaba `player1Holder`/`player2Holder` y resolvía "el otro" con un ternario.

### Portals (`Gameplay/Portals`)

| Componente | Responsabilidad |
|---|---|
| `Portal` | superficie, normal, portal enlazado |
| `PortalTraveler` | marcador en lo que puede viajar (jugador, disco) |
| `PortalTeleport` | transforma posición, rotación y velocidad; cooldown por viajero |
| `PortalPlacer` | raycast + validación contra `PortalRules` |

Todas las reglas que esperamos cambiar están en `PortalRules` (ScriptableObject):
cuándo se puede colocar, sobre qué superficies, distancias mínimas, quién puede atravesar,
conservación de momentum y cooldown anti-loop. Probar una variante es duplicar el asset.

### Goals (`Gameplay/Goals`)
`GoalZone` (trigger + dueño) levanta `GoalScored`. `GoalRelocator` las reubica sobre
segmentos definidos en la escena por `GoalPlacementLines`.

### Match (`Gameplay/Match`)
`MatchController` es la máquina de estados
(`Preparation → Countdown → Playing → GoalScored → Playing → Ended`) y el único que
arranca el reloj y congela a los jugadores. `PlayerRegistry` guarda los jugadores
registrados en la partida — nada de `players[2]`. `ScoreBoard` puntúa por identidad.

Es un coordinador, no un singleton: se referencia explícitamente.

### Presentation (`Game.Presentation`)
`GameplayEventLogger` (debug), y más adelante `DiscVfxBinder`, `MatchAudioBinder`,
`HudBinder`, `SharedCameraRig`. Cada uno se suscribe al canal y traduce a partículas,
sonido, UI o sacudida de cámara.

Prueba de fuego: borrar la carpeta entera debe dejar el juego jugable y feo.

## Física

- Plano de juego XY en Z = 0; `Rigidbody` con `FreezePositionZ | FreezeRotation`.
  La restricción está aislada en `PlanarMovement`, así que pasar a arena 3D abierta
  sería cambiar un componente, no el juego.
- Layers: `Player`, `Disc`, `Ground`, `Wall`, `PortalSurface`, `Portal`, `Goal`.
- La matriz de colisiones desactiva `Player ↔ Disc` (el disco no choca con cuerpos; la
  recogida es un `OverlapSphere` explícito) y aísla los triggers de `Portal` y `Goal`.
  El original hacía esto con `Physics2D.IgnoreCollision` a mano y tenía que reaplicarlo
  en cada respawn.
- Rebotes: material físico `Bouncy_Disc` para la respuesta base, más un multiplicador de
  velocidad en `DiscFlight` para la escalada, con tope de velocidad.

## Qué evitamos, y por qué

| Patrón | Motivo |
|---|---|
| `X.Instance` | dependencias invisibles; imposible tener dos partidas o una escena de test |
| `FindObjectsByType` en runtime | coste y acoplamiento oculto al orden de carga |
| Tags como arquitectura | strings sin validación; layers hacen el mismo trabajo con la matriz |
| Componentes de 300 líneas | imposibles de reemplazar por separado |
| Lógica que instancia partículas/sonido | impide cambiar el aspecto sin tocar el juego |
