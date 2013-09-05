using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class NutritionEffect : EffectInstance
{
    IntegerValue strength;

    public override void apply()
    {
        base.apply();
        npc.AdjustHunger(strength);
    }
}
