using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SpellAspect
{
    public ITargeter Targeter { get; set; }
    public List<EffectInstance> Effects { get; set; }

    public void Activate(SpellCastInfo castInfo)
    {
        foreach (IAffectable target in Targeter.GetTargets(castInfo))
            foreach (EffectInstance effect in Effects)
                effect.ActivateOnObject(target);
    }
}
