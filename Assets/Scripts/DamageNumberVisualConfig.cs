using UnityEngine;

[CreateAssetMenu(fileName = "DamageNumberVisualConfig", menuName = "Magical Tower/Visuals/Damage Number")]
public class DamageNumberVisualConfig : VisualConfig
{
    [SerializeField, Min(0.1f)] private float lifetime = 0.75f;
    [SerializeField] private Vector3 riseVelocity = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private Vector3 spawnOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private Color defaultColor = new Color(1f, 0.95f, 0.2f, 1f);

    public float Lifetime => lifetime;
    public Vector3 RiseVelocity => riseVelocity;
    public Vector3 SpawnOffset => spawnOffset;
    public Color DefaultColor => defaultColor;

    public override Visual GetConfiguredVisual()
    {
        return new DamageNumberVisual(this);
    }
}

public class DamageNumberVisual : Visual
{
    public float Amount { get; private set; }
    public Color Color { get; private set; }
    public override float Lifetime => ((DamageNumberVisualConfig)Config).Lifetime;

    public DamageNumberVisual(DamageNumberVisualConfig config) : base(config) { }

    public override void Initialize(VisualSpawnData data)
    {
        base.Initialize(data);
        Position = data.Position + ((DamageNumberVisualConfig)Config).SpawnOffset;
        Amount = data.Amount;
        Color = data.Color;
    }

    public override void Tick(float deltaTime)
    {
        Position += ((DamageNumberVisualConfig)Config).RiseVelocity * deltaTime;
        base.Tick(deltaTime);
    }
}
