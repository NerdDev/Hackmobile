using System;
using System.Collections.Generic;
using XML;

public abstract class DamageEffect : EffectInstance
{
    protected int strength;

    protected override void ParseParams(XMLNode node)
    {
        strength = node.SelectInt("strength");
    }
}
