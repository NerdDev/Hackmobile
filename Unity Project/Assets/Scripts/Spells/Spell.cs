using System;
using System.Collections.Generic;
using XML;

class Spell
{
    public TargetMethod method;
    public List<EffectInstance> effects = new List<EffectInstance>();
    //graphical effects?

    public void activate()
    {
        foreach (IAffectable wo in method.getTargets())
        {
            foreach (EffectInstance eb in effects)
            {
                eb.ActivateOnObject(wo);
            }
        }
    }

    public void parseXML()
    {

    }
}
