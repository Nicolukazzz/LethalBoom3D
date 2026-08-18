# Roadmap

Cada milestone termina en **algo que se puede abrir en Unity y jugar**. No medimos líneas
de código ni número de scripts.

Supuesto de capacidad: 3 personas a ~15 h/semana + Claude Code. Las semanas son de
calendario, no ideales. Si el equipo pasa a tiempo completo, dividir aproximadamente entre 2.

| Milestone | Resultado verificable | Estimación |
|---|---|---|
| **M0 Foundation** | Clone limpio abre, compila, consola sin errores; sandbox con arena greybox | 1–2 semanas |
| **M1 Core 3D** | Dos jugadores se mueven, se pasan un disco físico, marcan gol y la partida se reinicia | 3–4 semanas |
| **M2 Game Identity** | Portales, robo, bomba y scoring completo. **Gate de diversión** | 5–7 semanas |
| **M3 Vertical Slice** | Una arena presentable con animaciones, VFX, audio, HUD y menú | 8–10 semanas |
| **M4 Production** | Contenido, balance, settings, gamepad, pausa, polish | 10–14 semanas |
| **M5 Public Demo** | Build estable + página de Steam + demo jugable por externos | 6–8 semanas |
| **M6 1.0** | Release candidate | 6–10 semanas |

**Total ≈ 10–14 meses a 1.0**, ≈ 6–8 meses a demo pública.

---

## M0 — Foundation *(actual)*

Proyecto limpio, git + LFS, `CLAUDE.md`, documentación, estructura de `Assets`, ensamblados,
layers y matriz de colisiones, Input System, escenas sandbox, ScriptableObjects de balance.

**Hecho cuando**: un `git clone` en otra carpeta abre en Unity 6000.5.8f1 sin errores y
`Sandbox_Movement` entra en Play.

## M1 — Core 3D

- `PlanarMovement` + `GroundCheck` + doble salto, afinado hasta que se sienta bien.
- `PlayerInputRouter` con dos jugadores en un teclado y con gamepad.
- Cámara compartida con encuadre dinámico (zoom según separación) — **empieza aquí**,
  es un problema de game feel, no de polish.
- Disco físico: lanzamiento, vuelo, rebotes, recogida.
- `DiscPossession` y apuntado con la aguja oscilante del original.
- Una portería simple y `MatchController` con el loop completo.

**Hecho cuando**: dos personas juegan una partida entera de un minuto y marcan goles.

## M2 — Game Identity

- Portales en 3D. **Timebox de 2 semanas**; si no funciona, variante degradada
  (portales fijos colocados por diseño, o de un solo uso).
- Robo por contacto e intercepción.
- Bomba, explosión, respawn.
- Scoring con bonus de bomba; reubicación de porterías.
- Game feel: sacudida de cámara, hit stop, tiempo de reacción.
- Prototipo A/B de apuntado (aguja contra aim libre).

**Gate**: playtest con gente de fuera del equipo. La pregunta es *¿sigue siendo divertido
en 3D?* Si la respuesta es no, se ajusta el concepto **aquí**, no en M4.

## M3 — Vertical Slice

Una arena con identidad visual, personajes provisionales con animaciones, VFX, audio, HUD
final, menú principal y loop completo. Debe parecer el principio de un juego comercial.

Al final de M3 se **congela el alcance de contenido**: número de arenas y personajes.

## M4 — Production

Resto de arenas, balance, UX, menú de settings (audio, resolución, pantalla completa,
controles), pausa, soporte de gamepad con reconexión, accesibilidad razonable, polish.
Aquí empieza también el trámite fiscal de Steam.

## M5 — Public Demo

Build estable y reproducible, onboarding/tutorial, página de Steam publicada (mínimo dos
semanas antes de cualquier lanzamiento), capturas, tráiler, QA, playtest externo.

## M6 — 1.0

Contenido final, balance final, performance con presupuestos concretos sobre el hardware
mínimo que definamos, QA, integración mínima de Steamworks, release candidate.

---

## Riesgos

| Riesgo | Impacto | Mitigación |
|---|---|---|
| El concepto no traduce a 3D | Alto | Gate explícito al final de M2, con playtest externo grabado |
| Portales caros y abusables | Alto | Timebox de 2 semanas; variante degradada preparada |
| Alcance de contenido (arenas, personajes, animaciones) | Alto | Congelar al final de M3 y no reabrir |
| Mocap propio con 4 cámaras | Medio | No bloquea gameplay; plan B = Mixamo. Ver `ANIMATION_PIPELINE.md` |
| Cámara compartida con dos jugadores rápidos | Medio | Se ataca en M1, no al final |
| Solo co-op local limita ventas | Medio | Evaluar Remote Play Together en M4 (declarativo, barato) |
| Trámite fiscal de Steam | Bajo pero bloqueante | Empezar en M4; puede tardar semanas |

## Control de alcance

Ante cualquier mecánica nueva propuesta, responder por escrito:

1. ¿Cuánto aporta al core loop?
2. ¿Cuánto cuesta implementar?
3. ¿Cuánto cuesta mantener?
4. ¿Cuánto contenido adicional exige (arte, audio, arenas)?
5. ¿Bloquea otras features?
6. ¿Ahora, después o nunca?

No es rechazar ideas: es proteger la capacidad de terminar el juego.
