using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class NutritionEffect : EffectDefinition
{
    public override void apply(NPC n, float strength)
    {
        base.apply(n, strength);
        n.AdjustHunger(strength);
    }
}
