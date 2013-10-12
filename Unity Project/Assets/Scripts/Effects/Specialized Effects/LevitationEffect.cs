using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Levitation : EffectInstance
{
    Float strength;

    public override void Init(NPC n)
    {
        n.verticalMove(strength);
    }

    public override void Remove(NPC n)
    {
        n.verticalMove(-strength);
    }

    public override void SetParams()
    {
        strength = Add<Float>("strength");
    }
}
