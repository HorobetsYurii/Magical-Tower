# Magical Tower

A small 3D prototype: a tower in the middle of the arena auto-casts spells at waves of enemies
walking toward it. Made in Unity with primitive shapes for art. The task was about code structure,
so that's what this file covers.

## Structure

The game logic is plain C# and doesn't touch Unity beyond `Vector3` and math. The Unity side is
just views (MonoBehaviours) and config assets (ScriptableObjects). `GameManager` wires everything
up on `Start` and ticks it each frame in a fixed order: spells cast, then projectiles, effects and
enemies advance, then the views copy whatever the models did.

Every system is built the same way:

- a config `ScriptableObject` that holds the numbers and creates the model,
- a plain C# model whose behaviour is in a `Tick` method,
- a `MonoBehaviour` view that draws it,
- a controller that owns the list and ticks / removes items.

Enemies, spells, projectiles, effects and cosmetic visuals all follow that shape, so once you've
read one you've read them all. Whatever a system needs at runtime (the tower, the other
controllers, the camera helper) is passed around in a single `GameplayContext`.

Views stay trivial: each mirrors its model's position by id and is pooled per prefab. Enemies,
projectiles and visuals differ only in what they draw, so they reuse the same view/pool controller
instead of three copies of it.

## Adding content

Behaviour lives on the model's `Tick`, so adding something is a subclass plus a config asset, with
nothing to register anywhere:

- Enemy: Default, Fast and Big are already one `MeleeEnemy` class with three configs (different
  health, speed and size). A new *behaviour* is a new `Enemy` subclass with its `EnemyConfig` and a
  prefab, referenced from `EnemySpawnConfig`.
- Spell: a `Spell` and `SpellConfig`; add the asset to the spell list on `GameManager`.
- Projectile, effect or visual: same pattern, referenced from whatever config spawns them. A
  projectile config, for instance, points at its on-hit effect and its explosion visual.

## Spawning and difficulty

`EnemySpawnConfig` is a list of time windows, each with a spawn interval, a per-tick count and a
weighted mix of enemy types. Difficulty just means adding later windows with shorter intervals and
heavier enemies.

Where enemies appear is handled by `ViewportService`: it projects the screen edges onto the ground
and spawns enemies just outside the visible area. This is needed because the camera is tilted, so a
fixed radius around the tower would spawn some enemies already on screen and others too far out. The
same service answers whether a point is on screen, which both spells use so they aim at the same
enemies.
