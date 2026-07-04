using UnityEngine;

[CreateAssetMenu(fileName = "ParabolicProjectileConfig", menuName = "Magical Tower/Projectiles/Parabolic")]
public class ParabolicProjectileConfig : ProjectileConfig
{
    [SerializeField, Min(0.1f)] private float speed = 10f;
    [SerializeField, Min(0.1f)] private float damage = 5f;
    [SerializeField, Min(0f)] private float arcHeight = 3f;
    [SerializeField, Min(0.05f)] private float hitRadius = 0.75f;

    public float Speed => speed;
    public float Damage => damage;
    public float ArcHeight => arcHeight;
    public float HitRadius => hitRadius;

    public override Projectile GetConfiguredProjectile()
    {
        return new ParabolicProjectile(this);
    }
}

public class ParabolicProjectile : Projectile
{
    private Enemy target;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float flightDuration;
    private float elapsed;

    private ParabolicProjectileConfig ParabolicConfig => (ParabolicProjectileConfig)Config;

    public ParabolicProjectile(ParabolicProjectileConfig config) : base(config) { }

    public override void Initialize(ProjectileSpawnData spawnData)
    {
        Position = spawnData.Position;
        startPosition = spawnData.Position;
        target = spawnData.Target;
        targetPosition = spawnData.TargetPosition;

        var distance = Vector3.Distance(startPosition, targetPosition);
        flightDuration = Mathf.Max(0.05f, distance / ParabolicConfig.Speed);
        elapsed = 0f;
    }

    public override void Tick(GameplayContext context, float deltaTime)
    {
        var config = ParabolicConfig;

        if (target != null && target.IsAlive)
        {
            targetPosition = target.Position;
        }

        elapsed += deltaTime;

        var t = Mathf.Clamp01(elapsed / flightDuration);
        var position = Vector3.Lerp(startPosition, targetPosition, t);
        position.y += Mathf.Sin(t * Mathf.PI) * config.ArcHeight;
        Position = position;

        if (t < 1f)
        {
            return;
        }

        if (target != null &&
            target.IsAlive &&
            Vector3.Distance(Position, target.Position) <= config.HitRadius)
        {
            DamageUtility.ApplyDamage(target, config.Damage, context.Visuals);
        }

        MarkExpired();
    }
}
