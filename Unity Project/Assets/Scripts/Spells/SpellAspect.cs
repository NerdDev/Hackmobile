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
                effect.ActivateOnObject(target);
    }
}
