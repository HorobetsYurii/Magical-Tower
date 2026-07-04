using UnityEngine;

[CreateAssetMenu(fileName = "FireballSpellConfig", menuName = "Magical Tower/Spells/Fireball")]
public class FireballSpellConfig : SpellConfig
{
    [SerializeField, Min(0.05f)] private float cooldown = 2f;
    [SerializeField] private ProjectileConfig projectileConfig;
    [SerializeField] private Vector3 castOffset = new Vector3(0f, 2.5f, 0f);

    public float Cooldown => cooldown;
    public ProjectileConfig ProjectileConfig => projectileConfig;
    public Vector3 CastOffset => castOffset;

    public override Spell GetConfiguredSpell()
    {
        return new FireballSpell(this);
    }
}

public class FireballSpell : Spell
{
    public FireballSpell(FireballSpellConfig config) : base(config) { }

    public override void Tick(GameplayContext context, float deltaTime)
    {
        var config = (FireballSpellConfig)Config;

        TickCooldown(deltaTime);
        if (!IsReady)
        {
            return;
        }

        // Target a random enemy that is actually on screen, so Fireball and Barrage stay consistent.
        var target = context.Enemies.GetRandomEnemy(enemy => enemy.IsAlive && context.Viewport.IsVisible(enemy));
        if (target == null)
        {
            return;
        }

        var startPosition = context.Tower.Position + config.CastOffset;
        var targetPosition = target.AimPosition;
        var direction = (targetPosition - startPosition).normalized;
        context.Projectiles.Spawn(
            config.ProjectileConfig,
            ProjectileSpawnData.Directed(startPosition, direction, targetPosition, target));

        StartCooldown(config.Cooldown);
    }
}
