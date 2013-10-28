using System;
using System.Collections.Generic;
using XML;

public class PoisonSlowEffect : PoisonEffect
{
    Integer strength;
    Float slowPercentage;

    public override void Apply(NPC n)
    {
        base.Apply(n);
    }

    public override void Init(NPC n)
    {
        base.Init(n);
        //adjust NPC speed here
        //adjust NPC health here
        BigBoss.Gooey.CreateTextPop(n.gameObject.transform.position, "Poisoned!", UnityEngine.Color.green);
    }

    public override void Remove(NPC n)
    {
        //adjust NPC speed here back to normal
        base.Remove(n);
    }

    public override void SetParams()
    {
        strength = Add<Integer>("strength");
        slowPercentage = Add<Float>("slow");
    }
}
