using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class NutritionEffect : EffectInstance
{
    int strength;

    public override void Apply(NPC n)
    {
        n.AdjustHunger(strength);
    }

    protected override void ParseParams(XMLNode x)
    {
        strength = x.SelectInt("strength");
    }
}
