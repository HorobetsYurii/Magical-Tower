using UnityEngine;

public abstract class SpellConfig : ScriptableObject
{
    public abstract Spell GetConfiguredSpell();
}

public abstract class Spell
{
    protected Spell(SpellConfig config)
    {
        Config = config;
    }

    public SpellConfig Config { get; }
    public float CooldownRemaining { get; private set; }
    public bool IsReady => CooldownRemaining <= 0f;

    public abstract void Tick(GameplayContext context, float deltaTime);

    protected void TickCooldown(float deltaTime)
    {
        if (CooldownRemaining > 0f)
        {
            CooldownRemaining = Mathf.Max(0f, CooldownRemaining - deltaTime);
        }
    }

    protected void StartCooldown(float cooldown)
    {
        CooldownRemaining = Mathf.Max(0f, cooldown);
    }
}
