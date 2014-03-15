using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SpellAspect
{
    public ITargeter Targeter;
    public List<EffectInstance> Effects;

    public void Activate(SpellCastInfo castInfo)
    {
        foreach (IAffectable target in Targeter.GetTargets(castInfo))
            foreach (EffectInstance effect in Effects)
                target.ApplyEffect(castInfo.Caster, effect); //effect.ActivateOnObject(target);
    }

    public int GetHash()
    {
        int hash = 3;
        foreach (EffectInstance effect in Effects)
        {
            hash += effect.GetHash() * 5;
        }
        return hash;
    }
}
