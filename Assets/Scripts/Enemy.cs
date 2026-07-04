using UnityEngine;

// Behaviour lives in Tick, so a new enemy type is just a subclass + a config asset — nothing to register.
public abstract class Enemy : IPositionable, IAttackable, IEntity
{
    public int Id { get; internal set; }
    public Vector3 Position { get; set; }
    public EnemyConfig Config { get; }
    public GameObject Prefab => Config.Prefab;
    public abstract IHealth Health { get; }
    public bool IsAlive => !Health.IsDead;
    public Vector3 AimPosition => Position + Vector3.up * Config.AimHeight;

    protected Enemy(EnemyConfig config)
    {
        Config = config;
    }

    public abstract void Tick(GameplayContext context, float deltaTime);
    public abstract float TakeDamage(float damage);
}

public interface IPositionable
{
    public Vector3 Position { get; set; }
}

// Default / Fast / Big enemies are all this one class configured with different stats.
public class MeleeEnemy : Enemy
{
    private readonly Health health;
    private float attackCooldownRemaining;

    private MeleeEnemyConfig MeleeConfig => (MeleeEnemyConfig)Config;
    public override IHealth Health => health;

    public MeleeEnemy(MeleeEnemyConfig config) : base(config)
    {
        health = new Health(config.MaxHealth);
    }

    public override void Tick(GameplayContext context, float deltaTime)
    {
        var tower = context.Tower;
        if (tower.Health.IsDead)
        {
            return;
        }

        var config = MeleeConfig;
        if (attackCooldownRemaining > 0f)
        {
            attackCooldownRemaining = Mathf.Max(0f, attackCooldownRemaining - deltaTime);
        }

        if (Vector3.Distance(Position, tower.Position) < config.AttackRange)
        {
            if (attackCooldownRemaining <= 0f)
            {
                DamageUtility.ApplyDamage(tower, config.AttackDamage, context.Visuals);
                attackCooldownRemaining = config.AttackCooldown;
            }
        }
        else
        {
            var direction = (tower.Position - Position).normalized;
            Position += direction * (config.Speed * deltaTime);
        }
    }

    public override float TakeDamage(float damage)
    {
        return health.TakeDamage(damage);
    }
}
