using System;
using XML;

public class CurePoison : EffectInstance
{
    public override void init()
    {
        base.init();
        npc.RemoveEffectIfExists<PoisonEffect>();
    }

    public override void SetParams()
    {
    }
    }
}
