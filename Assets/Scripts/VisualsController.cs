using System.Collections.Generic;
using UnityEngine;

// Owns cosmetic entities. Spawn any VisualConfig; SpawnDamageNumber is a convenience since damage
// numbers are produced from all over the game.
public class VisualsController
{
    private readonly List<Visual> visuals = new List<Visual>();
    private readonly DamageNumberVisualConfig damageNumberConfig;
    private int nextVisualId = 1;

    public IReadOnlyList<Visual> Visuals => visuals;

    public VisualsController(DamageNumberVisualConfig damageNumberConfig)
    {
        this.damageNumberConfig = damageNumberConfig;
    }

    public Visual Spawn(VisualConfig config, VisualSpawnData data)
    {
        // Visuals are cosmetic: a missing config must never break gameplay.
        if (config == null)
        {
            return null;
        }

        var visual = config.GetConfiguredVisual();
        visual.Id = nextVisualId++;
        visual.Initialize(data);
        visuals.Add(visual);
        return visual;
    }

    public void SpawnDamageNumber(float amount, Vector3 position)
    {
        if (damageNumberConfig == null)
        {
            return;
        }

        SpawnDamageNumber(amount, position, damageNumberConfig.DefaultColor);
    }

    public void SpawnDamageNumber(float amount, Vector3 position, Color color)
    {
        if (amount <= 0f || damageNumberConfig == null)
        {
            return;
        }

        Spawn(damageNumberConfig, VisualSpawnData.DamageNumber(position, amount, color));
    }

    public void Tick(float deltaTime)
    {
        for (var i = visuals.Count - 1; i >= 0; i--)
        {
            var visual = visuals[i];
            if (visual.IsExpired)
            {
                visuals.RemoveAt(i);
                continue;
            }

            visual.Tick(deltaTime);

            if (visual.IsExpired)
            {
                visuals.RemoveAt(i);
            }
        }
    }
}
