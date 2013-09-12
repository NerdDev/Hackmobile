using System;
using System.Collections.Generic;
using XML;

public class EffectEvent : List<EffectInstance>
{
    public void activate(NPC wo)
    {
        // Disabled these because BigBoss's Log doesn't exist.
        // If these compile for you, make sure you push the BigBoss change
        // that contains Log so it compiles.
        //BigBoss.Log(this.Dump());
        foreach (EffectInstance e in this)
        {
            //BigBoss.Log(e.ToString());
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
                    this.Add(instance);
                }
                catch
                {
                }
            }
        }
    }
}
