# ADR-0001 — Perspectiva: 2.5D lateral con física 3D
2026-08-18 · Estado: aceptada

## Problema

El original es un plataformas 2D de vista lateral: correr, doble salto, disco que rebota en
paredes y techo, porterías en los muros. Pasarlo a 3D obliga a elegir cámara y espacio de
movimiento, y esa elección condiciona movimiento, apuntado, portales y legibilidad.

## Opciones consideradas

1. **Arena 3D en tercera persona** con pantalla partida. Máxima expresividad, pero cambia
   el juego entero: apuntar en 3D, dos cámaras, legibilidad del disco mucho peor, y
   pantalla partida en un juego de reacción es un problema serio.
2. **2.5D lateral con física 3D.** Movimiento en el plano XY, profundidad decorativa, una
   sola cámara compartida. El gameplay probado se conserva; el 3D aporta presentación,
   animaciones y mejores rebotes.
3. **Top-down / isométrico.** Buena legibilidad para dos jugadores, pero elimina la
   verticalidad y el salto, que son la mitad del movimiento actual.

## Decisión

Opción 2: **2.5D lateral con física 3D**, cámara única compartida.

## Razón

El riesgo grande del proyecto es que el concepto deje de ser divertido al cambiar de
dimensión. La opción 2 conserva lo que ya sabemos que funciona (posición relativa legible,
apuntado en un plano, rebotes previsibles) y gasta el presupuesto de riesgo en las cosas
que sí queremos mejorar: presentación, animaciones, portales y game feel.

## Consecuencias

- `Rigidbody` con `FreezePositionZ | FreezeRotation`; la restricción vive **solo** en
  `PlanarMovement`, así que abrir el juego a 3D completo sería sustituir un componente.
- Una sola cámara para los dos jugadores: hace falta encuadre dinámico (zoom según la
  separación) y es un problema de M1, no de polish.
- Los portales se colocan sobre superficies del plano; la orientación en profundidad queda
  fijada, lo que simplifica mucho su matemática.
- Las animaciones deben leerse bien desde un ángulo casi fijo: menos coste de mocap.
