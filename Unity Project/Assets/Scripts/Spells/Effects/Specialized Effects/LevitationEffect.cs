using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Levitation : EffectInstance
{
    float strength;

    public override void Init(NPC n)
    {
        n.verticalMove(strength);
    }

    public override void Remove(NPC n)
    {
        n.verticalMove(-strength);
    }

    protected override void ParseParams(XML.XMLNode x)
    {
        strength = x.SelectFloat("strength");
    }
}
