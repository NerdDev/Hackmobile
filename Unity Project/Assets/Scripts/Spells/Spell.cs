using System;
using System.Collections.Generic;
using System.Linq;
using XML;

public class Spell : Field
{
<<<<<<< HEAD
    SpellCastInfo info;
    protected SpellCastInfo CastInfo
    {
        get
        {
            if (info == null)
                info = new SpellCastInfo(aspects);
            return info;
        }
=======
    protected List<SpellAspect> aspects = new List<SpellAspect>();
    //graphical effects?

    public void activate()
    {
        //foreach (IAffectable wo in method.getTargets())
        //{
        //    foreach (EffectInstance eb in effects)
        //    {
        //        eb.ActivateOnObject(wo);
        //    }
        //}
>>>>>>> 68b455b8c47a1c99c269085df3d7ef3dc7ce043b
    }
    List<SpellAspect> aspects = new List<SpellAspect>();

    public void Activate(IAffectable caster)
    {
<<<<<<< HEAD
        Activate(new SpellCastInfo(caster));
    }

    public void Activate(IAffectable caster, params GridSpace[] space)
    {
        Activate(new SpellCastInfo(caster) { TargetSpaces = space });
    }

    public void Activate(IAffectable caster, params IAffectable[] targets)
    {
        Activate(new SpellCastInfo(caster) { TargetObjects = targets });
    }

    public void Activate(SpellCastInfo castInfo)
    {
        foreach (SpellAspect aspect in aspects)
            aspect.Activate(castInfo);
    }

    public SpellCastInfo GetCastInfoPrototype(IAffectable caster)
    {
        return new SpellCastInfo(caster, CastInfo);
    }

    public void parseXML(XMLNode topNode, string name)
    {
        XMLNode xnode = XMLNifty.select(topNode, name);

        // If no targeter specified, assume self
        AddAspect(new Self(), GetEffects(XMLNifty.SelectList(xnode, "effect")));

        foreach (XMLNode targeter in XMLNifty.SelectList(xnode, "targeter"))
        {
            string targeterType = XMLNifty.SelectString(targeter, "type");
            AddAspect(BigBoss.Types.Instantiate<ITargeter>(targeterType), 
                GetEffects(XMLNifty.SelectList(targeter, "effect")));
        }
    }

    protected void AddAspect(ITargeter targeter, List<EffectInstance> effects)
    {
        if (effects.Count == 0 || targeter == null)
            return;
        aspects.Add(new SpellAspect() { Targeter = targeter, Effects = effects });
    }

    protected List<EffectInstance> GetEffects(List<XMLNode> effects)
    {
        List<EffectInstance> ret = new List<EffectInstance>();
        foreach (XMLNode effect in effects)
        {
            string type = XMLNifty.SelectString(effect, "type");
            EffectInstance instance;
            if (BigBoss.Types.TryInstantiate(type, out instance))
            {
                instance.effect = type;
                instance.parseXML(effect);
                ret.Add(instance);
            }
            else if (BigBoss.Debug.logging(Logs.XML))
                BigBoss.Debug.log(Logs.XML, "Effect didn't exist: " + type);
        }
        return ret;
    }
=======
        //foreach (EffectInstance e in effects)
        //{
        //    e.ActivateOnObject(wo);
        //}
    }

    //public void Add(EffectInstance instance)
    //{
    //    effects.Add(instance);
    //}

    //public void parseXML(XMLNode topNode, string name)
    //{
    //    XMLNode xnode = XMLNifty.select(topNode, name);
    //    List<XMLNode> nodes = XMLNifty.SelectList(xnode, "effect");
    //    if (nodes != null)
    //    {
    //        foreach (XMLNode x in nodes)
    //        {
    //            string type = XMLNifty.SelectString(x, "type");
    //            try
    //            {
    //                EffectInstance instance = (EffectInstance)Activator.CreateInstance(EffectManager.effects[type]);
    //                instance.effect = type;
    //                instance.parseXML(x);
    //                this.Add(instance);
    //            }
    //            catch
    //            {
    //            }
    //        }
    //    }
    //}
>>>>>>> 68b455b8c47a1c99c269085df3d7ef3dc7ce043b
}
