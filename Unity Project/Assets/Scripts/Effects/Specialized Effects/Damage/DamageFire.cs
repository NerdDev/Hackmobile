using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DamageFire : DamageEffect
{
    public override void apply()
    {
        base.apply();
        if (!npc.HasEffect<FireResistance>())
        {
            npc.AdjustHealth(Convert.ToInt32(-strength));
        }
    }
}