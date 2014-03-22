using System;
using System.Collections.Generic;
using XML;

public abstract class DamageEffect : EffectInstance
{
    protected Damage strength;

    protected override void ParseParams(XMLNode node)
    {
        strength = node.Select<Damage>("strength");
    }
}
