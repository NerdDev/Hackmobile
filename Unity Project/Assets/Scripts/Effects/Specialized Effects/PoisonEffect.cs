using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PoisonEffect : EffectDefinition
{
    public override void apply(NPC n, float strength)
    {
        base.apply(n, strength);
        n.AdjustHealth(Convert.ToInt32(-strength));
    }

    public override void init(NPC n, float strength)
    {
        base.init(n, strength);
        BigBoss.Gooey.CreateTextPop(n.gameObject.transform.position, "Poisoned!", UnityEngine.Color.green);
    }
}