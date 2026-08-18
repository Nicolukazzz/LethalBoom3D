# Playtesting

El playtest no es una fase final: es parte del ciclo de cada mecánica.

```
implementar -> jugar -> observar -> recoger feedback -> decidir -> ajustar
```

## Reglas

- Cuando una mecánica sea polémica, **haz una variante y pruébala**. Media hora de
  prototipo vale más que dos horas de discusión.
- Prueba en **build**, no solo en el editor, al menos una vez por milestone.
- Observa, no expliques. Si hay que explicar cómo se juega, ese es el hallazgo.
- Anota lo que pasó, no lo que crees que pasó.

## Plantilla de sesión

Copia esto en `docs/playtests/YYYY-MM-DD.md` (crea la carpeta cuando haga falta).

```markdown
# Playtest YYYY-MM-DD
Build / commit:
Jugadores (nº, experiencia previa):
Qué queríamos aprender:

## Observaciones
- 

## Citas de los jugadores
- 

## Decisiones tomadas
- 

## Pendiente
- 
```

## Qué queremos medir

Estos eventos ya existen (o existirán) en `GameplayEventChannel`, así que registrarlos
más adelante **no requiere tocar gameplay**, solo añadir un listener en `Presentation/`:

| Métrica | Evento fuente |
|---|---|
| Duración real de la partida | `MatchPhaseChanged` |
| Goles por partida y por jugador | `GoalScored` |
| Tiempo de posesión por jugador | `DiscPossessionChanged` |
| Lanzamientos y velocidad | `DiscLaunched` |
| Robos e intercepciones | `DiscPossessionChanged` (`PossessionCause`) |
| Uso real de portales | `PortalTraversed`, `PortalPlaced` |
| Explosiones | `DiscExploded` |
| Bonus de bomba conseguido | `GoalScored.BombTimeHeldAtLaunch` |

**No implementes analytics todavía.** Lo importante ahora es que cada mecánica nueva
levante su evento.

## Registro de bugs

Un archivo `docs/BUGS.md` con cuatro niveles, creado cuando haya el primero:

- **blocker** — impide jugar o compilar. Se arregla antes que cualquier feature.
- **major** — rompe una mecánica o el flujo de partida.
- **minor** — molesto pero jugable.
- **polish** — estético o de sensación.
