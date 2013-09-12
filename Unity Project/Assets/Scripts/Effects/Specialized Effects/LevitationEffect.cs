using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Levitation : EffectInstance
{
    Float strength;

    public override void init()
    {
        base.init();
        npc.verticalMove(strength);
    }

    public override void remove()
    {
        base.remove();
        npc.verticalMove(-strength);
    }

    public override void SetParams()
    {
        strength = Add<Float>("strength");
    }
}
