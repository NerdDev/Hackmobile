using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DamageShock : DamageEffect
{
    public override void Apply(NPC n)
    {
        base.Apply(n);
        if (!n.HasEffect<ShockResistance>())
        {
            n.AdjustHealth(Convert.ToInt32(-strength));
        }
    }
}
