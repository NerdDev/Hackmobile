using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SpellAspect
{
<<<<<<< HEAD
    public ITargeter Targeter { get; set; }
    public List<EffectInstance> Effects { get; set; }

    public void Activate(SpellCastInfo castInfo)
    {
        foreach (IAffectable target in Targeter.GetTargets(castInfo))
            foreach (EffectInstance effect in Effects)
                effect.ActivateOnObject(target);
=======
    TargetMethod targeting;
    List<EffectInstance> effects = new List<EffectInstance>();

    public SpellAspect(TargetMethod targeting)
    {
        this.targeting = targeting;
>>>>>>> 68b455b8c47a1c99c269085df3d7ef3dc7ce043b
    }
}
