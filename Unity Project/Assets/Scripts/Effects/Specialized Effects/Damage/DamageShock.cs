using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DamageShock : DamageEffect
{
    public override void apply()
    {
        base.apply();
        if (!npc.HasEffect<ResistanceShock>())
        {
            npc.AdjustHealth(Convert.ToInt32(-strength));
        }
    }
}