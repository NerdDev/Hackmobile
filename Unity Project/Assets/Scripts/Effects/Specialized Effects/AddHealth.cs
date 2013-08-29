using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AddHealth : EffectDefinition
{
    public override void apply(NPC n, float strength)
    {
        base.apply(n, strength);
        n.AdjustHealth(Convert.ToInt32(strength));
    }
}
