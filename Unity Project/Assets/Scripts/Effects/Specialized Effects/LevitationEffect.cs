using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LevitationEffect : EffectInstance
{
    Float strength;

    public override void init()
    {
        base.init();
        npc.verticalMove(strength);
    }

    public override void remove()
    {
        base.apply();
        npc.verticalMove(-strength);
    }

    public override void SetParams()
    {
        strength = Add<Float>("strength");
    }
}
