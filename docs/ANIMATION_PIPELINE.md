# Animation Pipeline

## Objetivo

Experimentar con un sistema de captura de movimiento propio: **4 cámaras alrededor del
actor** y MediaPipe para extraer poses desde varias perspectivas, reconstruyendo un
esqueleto 3D utilizable como animación en Unity.

## Regla que no se negocia

**El gameplay no depende de este sistema.** Unity solo recibe `AnimationClip` finales.
Si mañana cambiamos MediaPipe por Blender, Rokoko, Mixamo, animación a mano o cualquier
otra cosa, el código de jugador no cambia una línea.

Plan B explícito: **Mixamo**. Si el experimento no da resultados usables antes de M3,
seguimos con Mixamo y punto.

## Pipeline previsto

```
Captura (4 cámaras)
  -> Sincronización temporal
  -> Calibración de cámaras (intrínsecos + extrínsecos)
  -> Detección de poses 2D (MediaPipe, por vista)
  -> Reconstrucción multi-vista / triangulación
  -> Filtrado y suavizado
  -> Reconstrucción de esqueleto
  -> Retargeting al rig del personaje
  -> Limpieza manual
  -> AnimationClip de Unity
```

No asumir que MediaPipe resuelve el proceso completo. Cada flecha es un problema con sus
propias trampas — sobre todo sincronización, calibración y jitter.

## Almacenamiento

Las capturas crudas (vídeo de las 4 cámaras) **nunca entran en Git**. Se guardan fuera del
repo; en Git solo entran los `AnimationClip` finales y el rig. Ver `.gitignore`.

## Experimentos

Aún ninguno. Registrar cada uno con este formato:

```markdown
### EXP-NN — título
Fecha:
Objetivo:
Montaje (cámaras, resolución, fps, iluminación):
Qué salió:
Problemas:
Decisión:
```

## Problemas conocidos por resolver

- Sincronizar 4 cámaras sin hardware de trigger (¿claqueta sonora? ¿flash?).
- Calibración extrínseca: se necesita un patrón visible desde varias vistas.
- Jitter de MediaPipe entre frames; el filtrado no debe comerse el movimiento rápido.
- Oclusión: con 4 cámaras hay puntos que ninguna ve bien.
- Escala y proporciones: el actor no mide lo que el personaje.
- Retargeting a un rig humanoide de Unity.

## Decisiones tomadas

Ninguna todavía. Cuando haya alguna, va a `docs/decisions/`.
