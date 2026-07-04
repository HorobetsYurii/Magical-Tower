using UnityEngine;

public class ExplosionView : VisualView
{
    protected override void OnSync(Visual value)
    {
        var explosion = (ExplosionVisual)value;
        // Reach full size within the first 30% of the lifetime so the blast reads as a fast pop.
        var grow = Mathf.Clamp01(value.Progress / 0.3f);
        var scale = explosion.Radius * 2f * Mathf.SmoothStep(0.5f, 1f, grow);
        transform.localScale = Vector3.one * scale;
    }
}
