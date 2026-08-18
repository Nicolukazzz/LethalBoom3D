# Gameplay

Dos partes: **cómo funcionaba el original** (auditoría de `GameJamEA`, que es la única
documentación real de esas mecánicas) y **qué conservamos** en el remake.

---

## Parte 1 — El original (GameJamEA)

Unity 6000.3.10, URP 2D, plataformas vista lateral, 2 jugadores en un teclado.
Clonado en `~/GameJamEA-reference`.

### Loop completo de una partida

1. **Preparación.** Los jugadores están congelados (`rb.simulated = false`). Cada uno
   coloca **2 portales** (`Q` / `P`): raycast desde el jugador hacia el ratón contra el
   layer `MapBounds`. No se puede colocar a menos de 2 u de una portería ni de un portal
   rival. Un tercer portal destruye el primero. Con `Space`, si ambos tienen sus 2
   portales, empieza la partida.
2. **Partida: 60 segundos.** Las porterías arrancan en los laterales, centradas.
3. **Posesión.** El disco empieza en manos de un jugador aleatorio. En mano: física
   desactivada, collider apagado, la posición se copia cada `LateUpdate` desde el
   `handTransform` del jugador.
4. **Bomba.** El contador solo corre **mientras alguien lo tiene en mano** (3 s). Al llegar
   a cero: el portador muere (respawn en su posición inicial tras 1.5 s) y **el disco pasa
   al rival**. La bomba no castiga esperar en el suelo, castiga acaparar.
5. **Apuntado y lanzamiento.** Primera pulsación entra en modo aim: el jugador queda
   inmóvil, no puede saltar y su gravedad baja a 0.4 (cámara lenta **local**, solo para
   quien apunta). Una aguja oscila en un cono de 180° hacia donde mira el personaje.
   Segunda pulsación lanza a velocidad fija en la dirección de la aguja.
6. **Vuelo y rebotes.** Cada rebote contra `Wall` o `Platform` multiplica la velocidad por
   1.15: los intercambios largos escalan solos. A los **8 rebotes** el disco se teleporta
   a la mano del rival.
7. **Intercepción.** El disco mide cada frame la distancia a cada jugador; a menos de
   0.8 u lo capturan. Quien lanzó tiene 0.3 s de inmunidad para no auto-recogerlo.
8. **Robo.** Chocar cuerpo a cuerpo con quien lleva el disco lo roba. La víctima sale
   empujada (velocidad limitada, 0.15 s sin control); el ladrón se queda quieto.
   Cooldown de 0.6 s. Si nadie lleva el disco, el choque solo empuja a los dos.
9. **Portales.** Trigger + componente `Teleportable`. La velocidad se descompone en normal
   y tangente del portal de entrada y se recompone en la base del de salida, así que el
   momentum se conserva. Sale 0.2 u por delante y queda inmune 0.1 s (anti-loop).
   Atraviesan tanto jugadores como disco.
10. **Gol.** Cada portería tiene dueño; el punto es para **el rival** del dueño. Cuenta el
    disco entrando en vuelo **y** el jugador que entra llevándolo en mano (autogol).
    Puntuación: **10 base + hasta 20 de bonus** proporcional al tiempo que el lanzador tuvo
    la bomba en mano al disparar. Es la mejor idea del diseño: premia soltar tarde.
11. **Tras el gol.** Las dos porterías se destruyen y reaparecen en posiciones aleatorias
    sobre techo y paredes (con margen anti-esquina y separación mínima de los portales).
    El disco desaparece 1 s y vuelve **quieto sobre una plataforma aleatoria**, libre para
    quien llegue primero.
12. **Fin.** A los 0 s todo se congela y se muestra el ganador.

### Números del original

| Parámetro | Valor |
|---|---|
| Duración de partida | 60 s |
| Temporizador de bomba | 3 s |
| Respawn tras explosión | 1.5 s |
| Velocidad de lanzamiento | 10 u/s |
| Multiplicador por rebote | ×1.15 |
| Rebotes máximos | 8 |
| Radio de intercepción | 0.8 u |
| Inmunidad del lanzador | 0.3 s |
| Cooldown de robo | 0.6 s |
| Velocidad de movimiento | 5 u/s |
| Salto / saltos máximos | 12 u/s, doble salto |
| Cámara lenta al apuntar | ×0.4 (solo quien apunta) |
| Puntos por gol | 10 + hasta 20 de bonus |
| Portales por jugador | 2 |
| Cancha | x ∈ [-10, 10], y ∈ [-5, 5] |

Estos valores están como defaults en los ScriptableObjects de `Assets/Game/Config`.
Son el **punto de partida**, no la verdad revelada.

---

## Parte 2 — El remake

### Qué conservamos (concepto, no implementación)

- Bomba cuyo temporizador solo corre en la mano.
- Bonus de puntuación por soltar tarde. **Es el corazón del juego.**
- Rebotes que aceleran el disco.
- Portales colocados por los jugadores como decisión estratégica.
- Porterías que se reubican tras cada gol.
- Robo por contacto e intercepción por proximidad.
- Partida corta con reloj.

### Qué rediseñamos

- **Posesión** fuera del disco, en su propio componente, sin conocer a jugadores concretos.
- **Apuntado** separado en input / resolución de dirección / visual, para poder probar
  aguja oscilante contra aim libre sin reescribir nada.
- **Portales** con sus reglas en un ScriptableObject (`PortalRules`), no constantes.
- **Estado de partida** como máquina de estados explícita con eventos, en vez de guardias
  `MatchEnded` repetidas por todo el código.
- **Rebotes y superficies** por layers, no por tags.
- **Muerte** sin desactivar el GameObject (rompía input y corrutinas).

### Qué está por decidir (prototipo en M2)

- Portales en 3D: dónde se pueden poner, orientación, tamaño, quién los atraviesa,
  antes o durante la partida, y si son legibles para los dos jugadores.
- Aim: aguja oscilante (habilidad de timing, telegrafiada para el rival) contra aim libre
  con stick (más directo). Se prueban las dos detrás de la misma interfaz.
- Qué pasa al llegar al límite de rebotes: teletransporte al rival (raro), caída al suelo,
  o pérdida progresiva de velocidad.
- Castigo por explosión: respawn completo o un aturdimiento más corto.
