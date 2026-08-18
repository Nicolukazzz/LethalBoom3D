# ADR-0002 — Separar gameplay de presentación con un canal de eventos
2026-08-18 · Estado: aceptada

## Problema

En el original, `Disc` llamaba a `Animator`, a `DiscParticles` y a `AudioManager.Instance`
directamente; `GameManager` recorría parámetros de `Animator` para resetear animaciones.
Cambiar el aspecto visual obligaba a tocar la mecánica, y probar la mecánica sin arte era
imposible.

## Opciones consideradas

1. **Llamadas directas** (lo actual). Simple, pero acopla lógica y presentación para siempre.
2. **UnityEvents en el inspector.** Visible para diseño, pero el cableado se rompe en
   silencio, no se puede seguir en código y ensucia los diffs de prefab.
3. **Canal de eventos como ScriptableObject.** Gameplay levanta eventos tipados; la
   presentación se suscribe. Se arrastra como asset, no es un singleton.
4. **Bus de eventos estático.** Cómodo, pero es un singleton con otro nombre: dependencias
   invisibles y estado global entre escenas de test.

## Decisión

Opción 3: `GameplayEventChannel` (ScriptableObject) con eventos C# tipados y payloads
`readonly struct`. Reforzado por los ensamblados: `Game.Gameplay` no puede referenciar
`Game.Presentation`.

## Razón

Es la opción que hace la regla **verificable por el compilador** en vez de por disciplina.
Además da gratis dos cosas que vamos a necesitar: escenas sandbox con su propio canal, y
un punto único donde enchufar métricas de playtest sin tocar el gameplay.

## Consecuencias

- Añadir una mecánica relevante implica añadir su evento al canal.
- La presentación nunca hace polling de estado si existe un evento.
- El canal limpia sus suscriptores en `OnEnable` porque los ScriptableObjects sobreviven a
  los reload del editor y arrastrarían listeners muertos.
- Prueba de fuego permanente: borrar `Assets/Game/Presentation/` debe dejar el juego
  jugable y feo, no roto.
