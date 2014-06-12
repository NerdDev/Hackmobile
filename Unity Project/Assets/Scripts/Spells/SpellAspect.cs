using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//unused
public class SpellAspect
{
    public Targeter Targeter;
    public List<EffectInstance> Effects;

    public void Activate(SpellCastInfo castInfo)
    {
        foreach (IAffectable target in Targeter.GetAffectableTargets(castInfo))
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
