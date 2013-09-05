using System;
using System.Collections.Generic;
using XML;

public class EffectEvent
{
    public List<EffectInstance> effects = new List<EffectInstance>();

    public void activate(NPC wo)
    {
        foreach (EffectInstance e in effects)
        {
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
                string type = XMLNifty.SelectString(xnode, "type");
                try
                {
                    EffectInstance instance = (EffectInstance)Activator.CreateInstance(EffectManager.effects[type]);
                    instance.effect = type;
                    instance.initialize(xnode);
                    effects.Add(instance);
                }
                catch
                {
                }
            }
        }
    }
}
