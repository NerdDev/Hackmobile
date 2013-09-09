using System;
using System.Collections.Generic;
using XML;

public class PoisonSlowEffect : PoisonEffect
{
    Integer strength;
    Float slowPercentage;

    public override void apply()
    {
        base.apply();
    }

    public override void init()
    {
        base.init();
        //adjust NPC speed here
        //adjust NPC health here
        BigBoss.Gooey.CreateTextPop(npc.gameObject.transform.position, "Poisoned!", UnityEngine.Color.green);
    }

    public override void remove()
    {
        //adjust NPC speed here back to normal
        base.remove();
    }

    public override void SetParams()
    {
        strength = Add<Integer>("strength");
        slowPercentage = Add<Float>("slow");
    }
}
