using System;
using System.Collections.Generic;
using UnityEngine;
using XML;

public class PoisonDamageEffect : PoisonEffect
{
    Integer strength;

    public override void apply()
    {
        base.apply();
        if (!npc.HasEffect<PoisonResistance>())
        {
            npc.AdjustHealth(Convert.ToInt32(-strength));
        }
    }

    public override void init()
    {
        base.init();
        npc.CreateTextPop("Poisoned!", Color.green);
    }

    public override void remove()
    {
        base.remove();
    }

    public override void SetParams()
    {
        strength = Add<Integer>("strength");
    }
}
