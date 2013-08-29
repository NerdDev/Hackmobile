using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class EffectDefinition
{
    public virtual void apply(NPC n, float strength)
    {
    }

    public virtual EffectInstance merge(EffectInstance firstInstance, EffectBase secondInstance)
    {
        return firstInstance;
    }

    public virtual void init(NPC n, float strength)
    {
    }

    public virtual void remove(NPC n, float strength)
    {
    }
}
