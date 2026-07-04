using UnityEngine;

public abstract class EffectConfig : ScriptableObject
{
    public abstract Effect GetConfiguredEffect();
}

public abstract class Effect
{
    protected Effect(EffectConfig config)
    {
        Config = config;
    }

    public EffectConfig Config { get; }
    public IAttackable Target { get; private set; }
    public bool IsExpired { get; private set; }

    public virtual void Initialize(IAttackable target)
    {
        Target = target;
    }

    public abstract void Tick(GameplayContext context, float deltaTime);

    protected void MarkExpired()
    {
        IsExpired = true;
    }
}
