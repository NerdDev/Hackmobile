using System;
using System.Collections.Generic;
using XML;

public class PoisonSlowEffect : PoisonEffect
{
    int strength;
    float slowPercentage;

    public override void Apply(NPC n)
    {
        base.Apply(n);
    }

    public override void Init(NPC n)
    {
        base.Init(n);
        //adjust NPC speed here
        //adjust NPC health here
        BigBoss.Gooey.CreateTextPop(n.GO.transform.position, "Poisoned!", UnityEngine.Color.green);
    }

    public override void Remove(NPC n)
    {
        //adjust NPC speed here back to normal
        base.Remove(n);
    }

    protected override void ParseParams(XMLNode x)
    {
        strength = x.SelectInt("strength");
        slowPercentage = x.SelectFloat("slow");
    }
}