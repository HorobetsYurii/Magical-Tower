using UnityEngine;

[CreateAssetMenu(fileName = "BurningEffectConfig", menuName = "Magical Tower/Effects/Burning")]
public class BurningEffectConfig : EffectConfig
{
    [SerializeField, Min(0.05f)] private float duration = 4f;
    [SerializeField, Min(0.05f)] private float tickInterval = 1f;
    [SerializeField, Min(0.1f)] private float damagePerTick = 3f;
    [SerializeField] private Color damageColor = new Color(1f, 0.5f, 0f, 1f);

    public float Duration => duration;
    public float TickInterval => tickInterval;
    public float DamagePerTick => damagePerTick;
    public Color DamageColor => damageColor;

    public override Effect GetConfiguredEffect()
    {
        return new BurningEffect(this);
    }
}

public class BurningEffect : Effect
{
    private float durationRemaining;
    private float tickTimer;

    public BurningEffect(BurningEffectConfig config) : base(config) { }

    public override void Initialize(IAttackable target)
    {
        base.Initialize(target);
        durationRemaining = ((BurningEffectConfig)Config).Duration;
        tickTimer = 0f;
    }

    public override void Tick(GameplayContext context, float deltaTime)
    {
        var config = (BurningEffectConfig)Config;

        if (Target.Health.IsDead)
        {
            MarkExpired();
            return;
        }

        durationRemaining -= deltaTime;
        tickTimer += deltaTime;

        while (tickTimer >= config.TickInterval)
        {
            tickTimer -= config.TickInterval;
            DamageUtility.ApplyDamage(Target, config.DamagePerTick, context.Visuals, config.DamageColor);
        }

        if (durationRemaining <= 0f)
        {
            MarkExpired();
        }
    }
}
