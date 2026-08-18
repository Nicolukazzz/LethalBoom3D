# Lethal Boom 3D

Juego local para 2 jugadores: cancha cerrada, un disco-bomba que explota si lo sostienes
demasiado tiempo, porterías que se reubican y portales colocados por los jugadores.
Remake 3D (2.5D lateral con física 3D) de [GameJamEA](https://github.com/Solaquin/GameJamEA).

**Nombre temporal.** El proyecto no depende de él.

## Requisitos

- **Unity 6000.5.8f1** (exacta; instalar desde Unity Hub)
- **Git LFS** — `sudo apt install git-lfs` / `brew install git-lfs`

## Clonar

```bash
git lfs install
git clone git@github.com:Nicolukazzz/LethalBoom3D.git
cd LethalBoom3D
```

Abre la carpeta con Unity Hub. Escena de trabajo actual:
`Assets/Game/Scenes/Sandbox_Movement.unity`.

Si al abrir aparecen texturas o modelos rotos, faltó `git lfs pull`.

## Merge de escenas y prefabs

Unity trae una herramienta de merge para sus archivos YAML. Configúrala una vez:

```bash
git config merge.unityyamlmerge.name "Unity SmartMerge"
git config merge.unityyamlmerge.driver '<ruta-unity>/Editor/Data/Tools/UnityYAMLMerge merge -p "$BASE" "$REMOTE" "$LOCAL" "$MERGED"'
```

En Linux la ruta suele ser `~/Unity/Hub/Editor/6000.5.8f1/Editor/Data/Tools/UnityYAMLMerge`.

## Estructura

```
Assets/Game/
  Core/         Eventos de gameplay, canal de eventos, constantes de layers
  Input/        Input System actions + PlayerInputRouter
  Gameplay/     Player, Disc, Portals, Goals, Match
  Presentation/ HUD, audio, VFX, cámara  (nunca referenciado desde Gameplay)
  Config/       ScriptableObjects de balance
  Prefabs/  Scenes/  Materials/  Models/  Audio/  Editor/
docs/           Visión, gameplay, arquitectura, roadmap, decisiones
```

## Documentación

| Documento | Para qué |
|---|---|
| `CLAUDE.md` | Reglas del proyecto y contexto para Claude Code |
| `docs/GAME_VISION.md` | Qué juego estamos haciendo y para quién |
| `docs/GAMEPLAY.md` | Mecánicas del original y del remake |
| `docs/ARCHITECTURE.md` | Cómo están organizados los sistemas |
| `docs/ROADMAP.md` | Milestones, estimaciones y riesgos |
| `docs/PLAYTESTING.md` | Cómo probamos y qué medimos |
| `docs/STEAM_RELEASE.md` | Checklist comercial |
| `docs/ANIMATION_PIPELINE.md` | Experimento de mocap propio |
| `docs/decisions/` | Decisiones técnicas y su porqué |

## Referencia del original

El juego de la jam está clonado en `~/GameJamEA-reference` (solo lectura, push deshabilitado).
Sirve como referencia de gameplay y game feel. **No se desarrolla sobre él.**
