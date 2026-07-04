using UnityEngine;

public class DamageNumberView : VisualView
{
    [SerializeField] private TextMesh label;

    protected override void OnBind(Visual value)
    {
        if (label == null)
        {
            throw new MissingReferenceException("Damage number label is not assigned.");
        }

        label.text = Mathf.CeilToInt(((DamageNumberVisual)value).Amount).ToString();
    }

    protected override void OnSync(Visual value)
    {
        if (ViewCamera != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - ViewCamera.transform.position);
        }

        var color = ((DamageNumberVisual)value).Color;
        color.a *= Mathf.Clamp01(1f - value.Progress);
        label.color = color;
    }
}
