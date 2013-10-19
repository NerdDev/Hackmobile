using System;
using XML;

public class CurePoison : EffectInstance
{
    public override void Init(NPC n)
    {
        n.RemoveAnEffect<PoisonEffect>();
    }

    public override void SetParams()
    {
    }
}
