using System;
using XML;

public class AddHealth : EffectInstance
{
    Float strength;

    public override void Apply(NPC n)
    {
        n.AdjustHealth(Convert.ToInt32(strength));
    }

    public override void SetParams()
    {
        strength = Add<Float>("strength");
    }
}
