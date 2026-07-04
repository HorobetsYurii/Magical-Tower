using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionVisualConfig", menuName = "Magical Tower/Visuals/Explosion")]
public class ExplosionVisualConfig : VisualConfig
{
    [SerializeField, Min(0.05f)] private float lifetime = 0.4f;

    public float Lifetime => lifetime;

    public override Visual GetConfiguredVisual()
    {
        return new ExplosionVisual(this);
    }
}

public class ExplosionVisual : Visual
{
    public float Radius { get; private set; }
    public override float Lifetime => ((ExplosionVisualConfig)Config).Lifetime;

    public ExplosionVisual(ExplosionVisualConfig config) : base(config) { }

    public override void Initialize(VisualSpawnData data)
    {
        base.Initialize(data);
        Radius = data.Radius;
    }
}
