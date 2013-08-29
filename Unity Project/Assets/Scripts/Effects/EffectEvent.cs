using System;
using System.Collections.Generic;
using XML;

public class EffectEvent
{
    public List<EffectBase> effects = new List<EffectBase>();

    public void activate(NPC wo)
    {
        foreach (EffectBase e in effects)
        {
            BigBoss.Log("Activating effect: " + e.effect + ".");
            e.activate(wo);
        }
    }

    public void parseXML(XMLNode x)
    {
        List<XMLNode> nodes = XMLNifty.SelectList(x, "effect");
        if (nodes != null)
        {
            foreach (XMLNode xnode in nodes)
            {
                EffectBase baseEffect = new EffectBase(xnode);
                effects.Add(baseEffect);
            }
        }
    }
}