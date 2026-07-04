using System;
using System.Collections.Generic;

public class EffectsController
{
    private readonly List<Effect> effects = new List<Effect>();

    public Effect Spawn(EffectConfig config, IAttackable target)
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        if (target == null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        var effect = config.GetConfiguredEffect();
        effect.Initialize(target);
        effects.Add(effect);
        return effect;
    }

    public void Tick(GameplayContext context, float deltaTime)
    {
        for (var i = effects.Count - 1; i >= 0; i--)
        {
            var effect = effects[i];
            if (effect.IsExpired)
            {
                RemoveEffectAt(i);
                continue;
            }

            effect.Tick(context, deltaTime);

            if (effect.IsExpired)
            {
                RemoveEffectAt(i);
            }
        }
    }

    private void RemoveEffectAt(int index)
    {
        effects.RemoveAt(index);
    }
}
