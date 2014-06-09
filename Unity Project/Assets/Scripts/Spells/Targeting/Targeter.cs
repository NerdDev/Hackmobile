using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Targeter : IXmlParsable
{
    public Targeter() { }

    internal TargetingStyle Style { get; set; }
    internal byte MaxTargets { get; set; }

    public List<EffectInstance> Effects;

    public virtual HashSet<IAffectable> GetAffectableTargets(SpellCastInfo castInfo)
    {
        return new HashSet<IAffectable>();
    }

    public virtual HashSet<GridSpace> GetGridTargets(SpellCastInfo castInfo)
    {
        return new HashSet<GridSpace>();
    }

    public virtual void Activate(IAffectable caster)
    {
        Activate(new SpellCastInfo(caster));
    }

    public virtual void Activate(SpellCastInfo castInfo)
    {
        foreach (IAffectable target in GetAffectableTargets(castInfo))
        {
            foreach (EffectInstance effect in Effects)
            {
                target.ApplyEffect(castInfo.Caster, effect); //effect.ActivateOnObject(target);
            }
        }
    }

    public virtual int GetHash()
    {
        int hash = 3;
        hash += Style.GetHashCode() * 13;
        hash += MaxTargets.GetHashCode() * 3;
        foreach (EffectInstance effect in Effects)
        {
            hash += effect.GetHash() * 5;
        }
        return hash;
    }

    public virtual void ParseXML(XMLNode x)
    {
        Effects = GetEffects(x.SelectList("effect"));
    }

    protected List<EffectInstance> GetEffects(IEnumerable<XMLNode> effects)
    {
        List<EffectInstance> ret = new List<EffectInstance>();
        foreach (XMLNode effect in effects)
        {
            string type = effect.SelectString("type");
            EffectInstance instance;
            if (BigBoss.Types.TryInstantiate(type, out instance))
            {
                instance.ParseXML(effect);
                instance.Name = type;
                ret.Add(instance);
            }
            else if (BigBoss.Debug.logging(Logs.XML))
                BigBoss.Debug.log(Logs.XML, "Effect didn't exist: " + type + " on node " + effect);
        }
        return ret;
    }
}
