using UnityEngine;

[CreateAssetMenu(fileName = "StraightProjectileConfig", menuName = "Magical Tower/Projectiles/Straight")]
public class StraightProjectileConfig : ProjectileConfig
{
    [SerializeField, Min(0.1f)] private float speed = 8f;
    [SerializeField, Min(0.1f)] private float damage = 15f;
    [SerializeField, Min(0.05f)] private float hitRadius = 0.35f;
    [SerializeField, Min(0.05f)] private float explosionRadius = 2f;
    [SerializeField, Min(0.1f)] private float maxLifetime = 4f;
    [SerializeField] private EffectConfig effectConfig;
    [SerializeField] private VisualConfig explosionVisual;

    public float Speed => speed;
    public float Damage => damage;
    public float HitRadius => hitRadius;
    public float ExplosionRadius => explosionRadius;
    public float MaxLifetime => maxLifetime;
    public EffectConfig EffectConfig => effectConfig;
    public VisualConfig ExplosionVisual => explosionVisual;

    public override Projectile GetConfiguredProjectile()
    {
        return new StraightProjectile(this);
    }
}

public class StraightProjectile : Projectile
{
    private Vector3 direction;
    private float lifetimeRemaining;

    private StraightProjectileConfig StraightConfig => (StraightProjectileConfig)Config;

    public StraightProjectile(StraightProjectileConfig config) : base(config) { }

    public override void Initialize(ProjectileSpawnData spawnData)
    {
        Position = spawnData.Position;
        direction = spawnData.Direction.normalized;
        lifetimeRemaining = StraightConfig.MaxLifetime;
    }

    public override void Tick(GameplayContext context, float deltaTime)
    {
        var config = StraightConfig;

        Position += direction * (config.Speed * deltaTime);
        lifetimeRemaining -= deltaTime;

        if (HitsAnyEnemy(context))
        {
            Explode(context);
            return;
        }

        if (TryFindGroundHit(out var groundPosition))
        {
            Position = groundPosition;
            Explode(context);
            return;
        }

        if (lifetimeRemaining <= 0f)
        {
            Explode(context);
        }
    }

    private bool HitsAnyEnemy(GameplayContext context)
    {
        foreach (var enemy in context.Enemies.Enemies)
        {
            if (enemy.IsAlive && IntersectsEnemy(Position, StraightConfig.HitRadius, enemy))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IntersectsEnemy(Vector3 projectilePosition, float projectileRadius, Enemy enemy)
    {
        var closestY = Mathf.Clamp(projectilePosition.y, enemy.Position.y, enemy.Position.y + enemy.Config.HitHeight);
        var closestPoint = new Vector3(enemy.Position.x, closestY, enemy.Position.z);
        var combinedRadius = projectileRadius + enemy.Config.HitRadius;
        return (projectilePosition - closestPoint).sqrMagnitude <= combinedRadius * combinedRadius;
    }

    private bool TryFindGroundHit(out Vector3 hitPosition)
    {
        hitPosition = Position;
        if (hitPosition.y > StraightConfig.HitRadius)
        {
            return false;
        }

        hitPosition.y = StraightConfig.HitRadius;
        return true;
    }

    private void Explode(GameplayContext context)
    {
        var config = StraightConfig;

        if (config.ExplosionVisual != null)
        {
            context.Visuals.Spawn(config.ExplosionVisual, VisualSpawnData.Effect(Position, config.ExplosionRadius));
        }

        foreach (var enemy in context.Enemies.Enemies)
        {
            if (!enemy.IsAlive)
            {
                continue;
            }

            if (Vector3.Distance(Position, enemy.Position) > config.ExplosionRadius)
            {
                continue;
            }

            var appliedDamage = DamageUtility.ApplyDamage(enemy, config.Damage, context.Visuals);
            if (appliedDamage > 0f && config.EffectConfig != null)
            {
                context.Effects.Spawn(config.EffectConfig, enemy);
            }
        }

        MarkExpired();
    }
}
