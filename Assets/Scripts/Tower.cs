using UnityEngine;

public class Tower : IAttackable
{
    private readonly Health health;

    public Tower(float maxHealth, Vector3 position)
    {
        health = new Health(maxHealth);
        Position = position;
    }

    public IHealth Health => health;
    public Vector3 Position { get; private set; }

    public float TakeDamage(float damage)
    {
        return health.TakeDamage(damage);
    }
}
