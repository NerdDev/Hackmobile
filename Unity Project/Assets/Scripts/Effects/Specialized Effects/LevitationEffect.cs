using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LevitationEffect : EffectDefinition
{
    public override void init(NPC n, float strength)
    {
        base.init(n, strength);
        n.verticalMove(strength);
    }

    public override void remove(NPC n, float strength)
    {
        base.apply(n, strength);
        n.verticalMove(-strength);
    }
}