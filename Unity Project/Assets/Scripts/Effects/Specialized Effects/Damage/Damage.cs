using System;
using System.Collections.Generic;
using XML;

public abstract class DamageEffect : EffectInstance
{
    protected int strength;

    protected override void parseXML(XMLNode x)
    {
        base.parseXML(x);
        strength = XMLNifty.SelectInt(x, "strength");
    }
}