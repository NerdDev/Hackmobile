using System;
using XML;

public class AddHealth : EffectInstance
{
    FloatValue strength;

    public override void apply()
    {
        base.apply();
        npc.AdjustHealth(Convert.ToInt32(strength));
    }

    protected override void parseXML(XMLNode x)
    {
        base.parseXML(x);
        strength = XMLNifty.SelectFloat(x, "strength");
    }
}
