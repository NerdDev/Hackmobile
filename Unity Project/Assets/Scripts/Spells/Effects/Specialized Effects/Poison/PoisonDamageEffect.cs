using System;
using System.Collections.Generic;

public class PoisonDamageEffect : PoisonEffect
{
    [Copyable] int strength;

    public override void Apply(NPC n)
    {
        base.Apply(n);
        if (!n.HasEffect<PoisonResistance>())
        {
            n.AdjustHealth(Convert.ToInt32(-strength));
        }
    }

    public override void Init(NPC n)
    {
        base.Init(n);
        BigBoss.Gooey.CreateTextMessage("Poisoned!", UnityEngine.Color.green);
    }

    public override void Remove(NPC n)
    {
        base.Remove(n);
    }

    protected override void ParseParams(XMLNode x)
    {
        strength = x.SelectInt("strength");
    }
}
