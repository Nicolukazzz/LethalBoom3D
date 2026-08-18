# CLAUDE.md — Lethal Boom 3D

Contexto principal para cualquier sesión de Claude Code en este repositorio.
Documento **estable**: si algo cambia cada semana, va en `docs/`, no aquí.

## Project Vision

Lethal Boom 3D es un juego local para 2 jugadores: una cancha cerrada, un **disco-bomba**
que explota si lo tienes demasiado tiempo en la mano, y porterías que se mueven tras cada gol.
Cada jugador coloca **portales** antes de la partida y los usa para redirigir el disco y a sí mismo.

Viene de `GameJamEA` (game jam, Unity 2D, repo `Solaquin/GameJamEA`, clonado en
`~/GameJamEA-reference` como **referencia de solo lectura**). Lo rehacemos porque queremos
un juego comercial, no un prototipo: el original cumplió su función pero su arquitectura
(3 singletons, `FindObjectsByType` en gameplay, `Player1`/`Player2` cableado, lógica y
presentación mezcladas) no aguanta meses de desarrollo.

Perspectiva del remake: **2.5D lateral con física 3D**. El juego ocurre en el plano XY
(Z = 0), la profundidad es decorativa y hay una única cámara compartida.
Ver `docs/decisions/ADR-0001-perspectiva-2_5d.md`.

Objetivo: **publicar en Steam**. Equipo de 3 personas + Claude Code.

## Core Principles

1. **Gameplay first.** Si no se puede jugar, no está hecho.
2. **Separar gameplay de presentación.** Borrar `Assets/Game/Presentation/` debe dejar el
   juego funcional y feo, nunca roto.
3. **Composición sobre herencia.** Componentes pequeños con una responsabilidad.
4. **Bajo acoplamiento, dependencias explícitas.** Referencias serializadas o eventos;
   nada de buscar objetos por nombre o tag.
5. **Iteración rápida.** Los valores que vamos a tocar en playtest son campos serializados
   o ScriptableObjects.
6. **Multijugador local ahora, sin cerrar puertas.** Nada de `Player1`/`Player2` global.
   **Sin networking**: no se implementa online en ningún milestone actual.
7. **Sin sobreingeniería.** Una abstracción necesita una razón concreta *hoy*.
8. **Rendimiento razonable.** Perfilar antes de optimizar.
9. **Código legible por otros dos humanos.** Nombres claros antes que listos.

## Architecture Rules

- **Ensamblados** (`.asmdef`) imponen las fronteras:
  `Game.Core` → `Game.Input` → `Game.Gameplay` → `Game.Presentation`.
  `Game.Presentation` referencia gameplay; **gameplay nunca referencia presentación**.
  El compilador vigila la regla por nosotros.
- **Sin singletons.** Nada de `X.Instance`. Coordinadores sí (`MatchController`), pero
  referenciados explícitamente o registrados en un `PlayerRegistry`.
- **Comunicación**: eventos C# directos entre componentes vecinos; entre sistemas,
  `GameplayEventChannel` (ScriptableObject, `Assets/Game/Config/GameplayEventChannel.asset`).
  Ese canal es también la futura fuente de métricas de playtest: cuando añadas una mecánica
  relevante, añade su evento.
- **Prohibido en código de gameplay**: `FindObjectOfType`/`FindObjectsByType` en runtime,
  `GameObject.Find`, `Camera.main` en `Update`, tags como arquitectura, `Shader.Find`.
  Las tags se sustituyen por **layers** (`GameLayers`) y componentes marcadores
  (`PortalTraveler`, `DiscCarrier`, …).
- **Input**: solo Input System. Cero `KeyCode`/`Input.GetKey` en el proyecto.
  El único punto de contacto es `PlayerInputRouter`.
- **Física 3D**: la matriz de colisiones de Project Settings es el contrato; no
  `Physics.IgnoreCollision` a mano. Movimiento en el plano XY con `Rigidbody` y
  `FreezePositionZ`.
- **Steam**: la lógica del juego no conoce Steamworks. Cuando llegue, va detrás de una capa
  fina y reemplazable, y el juego debe arrancar sin Steam abierto.

## Unity Rules

- **Escenas**: `Assets/Game/Scenes/`. `Bootstrap` (entrada), `Sandbox_*` (una por sistema,
  para probar aislado). Evitar una escena gigante: el contenido vive en **prefabs** y la
  arena se carga aditivamente. Tres personas editando la misma escena = conflictos.
- **Prefabs**: `Assets/Game/Prefabs/<sistema>/`. Todo lo instanciable es prefab.
- **ScriptableObjects**: para balance (`MatchConfig`, `DiscConfig`, `PlayerMovementConfig`,
  `PortalRules`), identidad de jugador (`PlayerProfile`) y el canal de eventos.
  No conviertas en SO algo que solo usa un componente.
- **Layers**: `Player`, `Disc`, `Ground`, `Wall`, `PortalSurface`, `Portal`, `Goal`
  (ver `GameLayers.cs`). Si añades una, actualiza la matriz de colisiones y la constante.
- **UI y audio** viven en `Presentation/` y se suscriben al canal de eventos. Nunca hacen
  polling de estado de gameplay en `Update` si existe un evento.
- **Arena greybox**: se genera con `Tools > LethalBoom > Build Greybox Arena` para que el
  layout sea revisable como código en vez de un diff de escena ilegible.
- **Assets binarios** grandes van por Git LFS (ver `.gitattributes`). Las capturas crudas de
  mocap **nunca** entran al repo.

## Git Workflow

- `main` es la rama estable y **siempre jugable**. Nunca se rompe a propósito.
- Ramas cortas: `feat/…`, `fix/…`, `refactor/…`, `polish/…` → Pull Request a `main`.
  Sin ramas permanentes extra.
- Commits pequeños y coherentes. Nunca mezclar en el mismo commit un refactor grande,
  una mecánica nueva y cambios visuales.
- Mensajes de commit en **inglés**; documentación y discusión en español.
- Antes de tocar una pieza central, mira quién más la usa: puede haber otra feature en vuelo.
  No hagas refactors globales sin acordarlo.

## Claude Code Rules

Antes de implementar una feature significativa:

1. Entender el sistema relacionado (leer el código existente, no asumir).
2. Revisar dependencias y quién consume lo que vas a tocar.
3. Proponer una solución **pequeña** y confirmarla.
4. Implementar.
5. Compilar.
6. Probar dentro del juego, no solo compilar.
7. Revisar warnings y errores de consola.
8. Actualizar documentación cuando corresponda (y `docs/decisions/` si la decisión
   costará entenderse dentro de tres meses).

No generes veinte sistemas anticipando problemas futuros. Implementa lo que el juego
necesita ahora con un diseño que permita evolucionar.

## Current Milestone

**M0 — Foundation (en curso).** Repo, git+LFS, estructura, asmdefs, layers y matriz de
colisiones, Input System, escenas sandbox, configs base y documentación.
Siguiente: **M1 — Core 3D**, empezando por `feat/planar-movement`.

Actualiza esta sección solo al cambiar de milestone. Detalle en `docs/ROADMAP.md`.

## Definition of Done

Una feature está terminada cuando:

- funciona dentro del juego, probada en Play Mode;
- sus referencias están configuradas en prefabs/escena (no "falta arrastrar algo");
- no genera errores ni warnings nuevos en consola;
- maneja sus casos básicos (sin jugador, sin disco, respawn, fin de partida);
- se entiende leyéndola: nombres claros y comentarios donde el *por qué* no es obvio;
- se integra con los sistemas existentes en lugar de duplicarlos;
- se puede probar aislada (escena sandbox o valores expuestos);
- no introduce dependencias globales ni búsquedas por nombre/tag.

Compilar **no** es estar terminado.
