using UnityEngine;

// A cosmetic entity (damage number, explosion...) that mirrors the projectile stack but never
// touches gameplay state — it only advances its own age and position.
public abstract class VisualConfig : ScriptableObject
{
    [SerializeField] private GameObject prefab;

    public GameObject Prefab => prefab;
    public abstract Visual GetConfiguredVisual();
}

public abstract class Visual : IPositionable, IEntity
{
    protected Visual(VisualConfig config)
    {
        Config = config;
    }

    public int Id { get; internal set; }
    public Vector3 Position { get; set; }
    public VisualConfig Config { get; }
    public GameObject Prefab => Config.Prefab;
    public bool IsExpired { get; private set; }
    public float Age { get; private set; }
    public abstract float Lifetime { get; }
    public float Progress => Lifetime <= 0f ? 1f : Mathf.Clamp01(Age / Lifetime);

    public virtual void Initialize(VisualSpawnData data)
    {
        Position = data.Position;
        Age = 0f;
    }

    public virtual void Tick(float deltaTime)
    {
        Age += deltaTime;
        if (Age >= Lifetime)
        {
            MarkExpired();
        }
    }

    protected void MarkExpired()
    {
        IsExpired = true;
    }
}

public readonly struct VisualSpawnData
{
    private VisualSpawnData(Vector3 position, float amount, float radius, Color color)
    {
        Position = position;
        Amount = amount;
        Radius = radius;
        Color = color;
    }

    public Vector3 Position { get; }
    public float Amount { get; }
    public float Radius { get; }
    public Color Color { get; }

    public static VisualSpawnData DamageNumber(Vector3 position, float amount, Color color)
    {
        return new VisualSpawnData(position, amount, 0f, color);
    }

    public static VisualSpawnData Effect(Vector3 position, float radius)
    {
        return new VisualSpawnData(position, 0f, radius, Color.white);
    }
}
