using System;
using System.Collections.Generic;
using XML;

public abstract class DamageEffect : EffectInstance
{
    protected int strength;

    public override void SetParams()
    {
        strength = Add<Integer>("strength");
    }
}
