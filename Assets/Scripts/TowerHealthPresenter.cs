using UnityEngine;
using UnityEngine.UI;

public class TowerHealthPresenter : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Text healthLabel;
    [SerializeField] private Transform worldFill;
    [SerializeField] private TextMesh worldLabel;

    private IHealth health;
    private Vector3 fullWorldFillScale;

    public void Bind(IHealth value)
    {
        health = value;
        if (worldFill != null)
        {
            fullWorldFillScale = worldFill.localScale;
        }

        Refresh();
    }

    private void LateUpdate()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (health == null)
        {
            return;
        }

        if (healthBar != null)
        {
            healthBar.value = health.Current / health.Max;
        }

        if (healthLabel != null)
        {
            healthLabel.text = $"{Mathf.CeilToInt(health.Current)} / {Mathf.CeilToInt(health.Max)}";
        }

        if (worldFill != null)
        {
            var scale = fullWorldFillScale;
            scale.x *= health.Current / health.Max;
            worldFill.localScale = scale;
        }

        if (worldLabel != null)
        {
            worldLabel.text = $"{Mathf.CeilToInt(health.Current)} / {Mathf.CeilToInt(health.Max)}";
        }
    }
}
