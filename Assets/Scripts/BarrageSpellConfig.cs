using UnityEngine;

[CreateAssetMenu(fileName = "BarrageSpellConfig", menuName = "Magical Tower/Spells/Barrage")]
public class BarrageSpellConfig : SpellConfig
{
    [SerializeField, Min(0.05f)] private float cooldown = 4f;
    [SerializeField] private ProjectileConfig projectileConfig;
    [SerializeField] private Vector3 castOffset = new Vector3(0f, 2.5f, 0f);

    public float Cooldown => cooldown;
    public ProjectileConfig ProjectileConfig => projectileConfig;
    public Vector3 CastOffset => castOffset;

    public override Spell GetConfiguredSpell()
    {
        return new BarrageSpell(this);
    }
}

public class BarrageSpell : Spell
{
    public BarrageSpell(BarrageSpellConfig config) : base(config) { }

    public override void Tick(GameplayContext context, float deltaTime)
    {
        var config = (BarrageSpellConfig)Config;

        TickCooldown(deltaTime);
        if (!IsReady)
        {
            return;
        }

        var startPosition = context.Tower.Position + config.CastOffset;
        var spawned = 0;
        foreach (var target in context.Enemies.Enemies)
        {
            if (!target.IsAlive || !context.Viewport.IsVisible(target))
            {
                continue;
            }

            context.Projectiles.Spawn(
                config.ProjectileConfig,
                ProjectileSpawnData.Targeted(startPosition, target));

            spawned++;
        }

        if (spawned > 0)
        {
            StartCooldown(config.Cooldown);
        }
    }
}
