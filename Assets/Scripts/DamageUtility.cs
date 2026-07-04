using UnityEngine;

public static class DamageUtility
{
    // All damage flows through here: apply it, then spawn a damage number at the target.
    public static float ApplyDamage(IAttackable target, float damage, VisualsController visuals)
    {
        var appliedDamage = target.TakeDamage(damage);
        visuals.SpawnDamageNumber(appliedDamage, target.Position);
        return appliedDamage;
    }

    // Colour override for the damage number (e.g. burning ticks in orange).
    public static float ApplyDamage(IAttackable target, float damage, VisualsController visuals, Color color)
    {
        var appliedDamage = target.TakeDamage(damage);
        visuals.SpawnDamageNumber(appliedDamage, target.Position, color);
        return appliedDamage;
    }
}
