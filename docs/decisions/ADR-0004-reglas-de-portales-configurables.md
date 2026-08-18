# ADR-0004 — Reglas de portales en un ScriptableObject
2026-08-18 · Estado: aceptada

## Problema

Los portales son la mecánica que más vamos a cambiar, y la que menos sabemos cómo debe
funcionar en 3D: dónde se pueden colocar, con qué orientación y tamaño, quién puede
atravesarlos, si se colocan antes o durante la partida, cuánto momentum conservan, cómo se
evitan los bucles y si son legibles para el rival.

En el original todo eso eran constantes repartidas entre `PortalPlacer` (distancias mínimas,
layer de superficie) y `Portal` (offset de salida, cooldown de 0.1 s hardcodeado en una
corrutina). Probar una variante era editar código.

## Opciones consideradas

1. **Campos serializados en los componentes.** Suficiente para un valor suelto, pero
   dispersa las reglas por varios prefabs y no permite tener dos configuraciones completas
   listas para comparar.
2. **Un ScriptableObject `PortalRules`** con todas las reglas.
3. **Sistema de reglas con interfaces y validadores componibles.** Flexible, y claramente
   sobreingeniería para lo que sabemos hoy.

## Decisión

Opción 2: un `PortalRules` que agrupa cuándo, dónde, quién y cómo.

## Razón

Convierte un experimento de diseño en "duplica el asset y cambia dos campos", que es
exactamente el ciclo que queremos en M2. Es una abstracción con una razón concreta:
sabemos que estas reglas van a cambiar varias veces.

## Consecuencias

- `PortalPlacer` y `PortalTeleport` leen del asset; no guardan constantes propias.
- Podemos llevar dos o tres variantes a un mismo playtest y cambiarlas entre partidas.
- Si una regla resulta ser definitiva, se puede volver a fijar en código sin drama.
- Riesgo asumido: un asset con muchos campos tiende a crecer. Si aparece una segunda
  responsabilidad clara (por ejemplo, aspecto visual del portal), se separa.
