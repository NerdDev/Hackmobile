using System;
using System.Collections.Generic;
using XML;

public class PoisonSlowEffect : PoisonEffect
{
    IntegerValue strength;
    FloatValue slowPercentage;
    public PoisonSlowEffect()
    {
        map.Add("strength", strength);
        map.Add("slow", slowPercentage);
    }

    public override void apply()
    {
        base.apply();
    }

    public override void init()
    {
        base.init();
        //adjust NPC speed here
        BigBoss.Gooey.CreateTextPop(npc.gameObject.transform.position, "Poisoned!", UnityEngine.Color.green);
    }

    public override void remove()
    {
        //adjust NPC speed here
        base.remove();
    }

    protected override void parseXML(XMLNode x)
    {
        base.parseXML(x);
        strength = XMLNifty.SelectInt(x, "strength");
        slowPercentage = XMLNifty.SelectFloat(x, "slow");
    }
}