using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class NutritionEffect : EffectInstance
{
    Integer strength;

    public override void apply()
    {
        base.apply();
        npc.AdjustHunger(strength);
    }

    public override void SetParams()
    {
        strength = Add<Integer>("strength");
    }
}
