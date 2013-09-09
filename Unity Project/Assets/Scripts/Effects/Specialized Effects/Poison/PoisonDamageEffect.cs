using System;
using System.Collections.Generic;
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
        BigBoss.Gooey.CreateTextPop(npc.gameObject.transform.position, "Poisoned!", UnityEngine.Color.green);
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
