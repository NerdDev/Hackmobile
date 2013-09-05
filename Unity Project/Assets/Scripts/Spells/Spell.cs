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
        foreach (WorldObject wo in method.getTargets())
        {
            if (wo is NPC)
            {
                NPC n = wo as NPC;
                foreach (EffectInstance eb in effects)
                {
                    eb.activate(n);
                }
            }
        }
    }

    public void parseXML()
    {

    }
}