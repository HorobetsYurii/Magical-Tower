using System;
using System.Collections.Generic;

public class SpellsController
{
    private readonly List<Spell> spells = new List<Spell>();

    public void AddSpell(SpellConfig config)
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        var spell = config.GetConfiguredSpell();
        spells.Add(spell);
    }

    public void Tick(GameplayContext context, float deltaTime)
    {
        foreach (var spell in spells)
        {
            spell.Tick(context, deltaTime);
        }
    }
}
