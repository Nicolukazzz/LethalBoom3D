# Steam Release

Checklist de lo que un prototipo ignora y un producto comercial no puede. **Nada de esto se
implementa todavía**; existe para que no descubramos que falta medio producto tres semanas
antes del lanzamiento.

## Regla de oro

La lógica del juego **no conoce Steamworks**. Cualquier integración va detrás de una capa
fina y reemplazable (`IPlatformServices` o similar) con una implementación nula por
defecto. Durante el desarrollo el juego arranca sin Steam abierto.

Nada de achievements, Cloud, estadísticas o presencia hasta que el juego los necesite.

## Trámites (empezar en M4 — tardan)

- [ ] Cuenta de Steamworks + **Steam Direct: 100 USD por juego** (recuperables)
- [ ] Datos fiscales y bancarios verificados — semanas, no días
- [ ] App ID asignado
- [ ] Revisión de contenido de Valve (días hábiles)
- [ ] Página de la tienda publicada **≥ 2 semanas antes** de cualquier lanzamiento

## Producto (M4–M6)

- [ ] Menú principal
- [ ] Pausa
- [ ] Settings: audio (master/música/SFX), resolución, pantalla completa/ventana, VSync
- [ ] Controles visibles y, si hace falta, remapping
- [ ] Soporte de gamepad, incluida **reconexión en caliente**
- [ ] Tutorial / onboarding — el juego se enseña solo o pierde jugadores
- [ ] UI final coherente
- [ ] Audio completo: música, SFX, mezcla
- [ ] Accesibilidad razonable: tamaño de texto legible, no depender solo del color,
      posibilidad de reducir sacudida de cámara
- [ ] Créditos
- [ ] Licencias de todos los assets de terceros documentadas en `docs/ASSET_LICENSES.md`

## Técnico

- [ ] Builds reproducibles (script de build, versión etiquetada)
- [ ] Sin errores en consola en el build
- [ ] Performance medida con Profiler contra un hardware mínimo **definido** (aún sin definir)
- [ ] QA sobre builds reales, no solo editor
- [ ] Manejo de errores: el juego no se cierra sin decir nada

## Marketing

- [ ] Capsule art (varios tamaños)
- [ ] 5+ capturas que muestren la mecánica, no menús
- [ ] Tráiler: los primeros 6 segundos deben mostrar el juego
- [ ] Descripción de la tienda y etiquetas
- [ ] Demo pública (M5)
- [ ] Wishlist activo antes del lanzamiento — es el único número que predice ventas

## Decisiones pendientes

- ¿Remote Play Together? Es declarativo y barato para un juego de sofá. Decidir en M4.
- ¿Precio? Comparables del género están entre 5 y 15 USD.
- ¿Idiomas? Como mínimo español e inglés; el juego tiene poco texto.
