using UnityEngine;

public class Health : IHealth
{
    public float Current { get; private set; }
    public float Max { get; }
    public bool IsDead => Current <= 0f;

    public Health(float max)
    {
        Max = Mathf.Max(1f, max);
        Current = Max;
    }

    public float TakeDamage(float damage)
    {
        if (damage <= 0f || IsDead)
        {
            return 0f;
        }

        var previous = Current;
        Current = Mathf.Max(0f, Current - damage);
        return previous - Current;
    }
}

public interface IHealth
{
    public float Current { get; }
    public float Max { get; }
    public bool IsDead { get; }
}

public interface IAttackable
{
    public IHealth Health { get; }
    public Vector3 Position { get; }

    public float TakeDamage(float damage);
}
