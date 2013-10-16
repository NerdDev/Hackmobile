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

    public void activate(NPC wo)
    {
        foreach (EffectInstance e in effects)
        {
            e.ActivateOnObject(wo);
        }
    }

    public void parseXML(XMLNode topNode, string name)
    {
        XMLNode xnode = XMLNifty.select(topNode, name);
        List<XMLNode> nodes = XMLNifty.SelectList(xnode, "effect");
        if (nodes != null)
        {
            foreach (XMLNode x in nodes)
            {
                string type = XMLNifty.SelectString(x, "type");
                try
                {
                    EffectInstance instance = (EffectInstance)Activator.CreateInstance(EffectManager.effects[type]);
                    instance.effect = type;
                    instance.parseXML(x);
                    this.Add(instance);
                }
                catch
                {
                }
            }
        }
    }
}
