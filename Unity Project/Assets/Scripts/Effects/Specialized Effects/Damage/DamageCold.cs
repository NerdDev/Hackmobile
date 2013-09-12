using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DamageCold : DamageEffect
{
    public override void apply()
    {
        base.apply();
        if (!npc.HasEffect<ColdResistance>())
        {
            npc.AdjustHealth(Convert.ToInt32(-strength));
        }
    }
}