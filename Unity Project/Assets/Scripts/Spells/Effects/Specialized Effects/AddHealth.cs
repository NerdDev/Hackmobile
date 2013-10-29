using System;
using XML;

public class AddHealth : EffectInstance
{
    public float Strength;

    public override void Apply(NPC n)
    {
        n.AdjustHealth(Convert.ToInt32(Strength));
    }

    protected override void ParseParams(XMLNode node)
    {
        Strength = node.SelectFloat("strength");
    }
}
