using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spell : IXmlParsable
{
    protected SpellCastInfo info;
    public string Icon = "";
    public int range = 0;
    public int cost = 0;
    protected SpellCastInfo CastInfo
    {
        get
        {
            if (info == null)
                info = new SpellCastInfo(targeters);
            return info;
        }
    }
    protected List<Targeter> targeters = new List<Targeter>();

    public void Activate(IAffectable caster)
    {
        Activate(new SpellCastInfo(caster));
    }

    public void Activate(IAffectable caster, params IAffectable[] targets)
    {
        Activate(new SpellCastInfo(caster) { TargetObjects = targets });
    }

    public void Activate(IAffectable caster, params Vector3[] locations)
    {
        Activate(new SpellCastInfo(caster) { TargetLocations = locations });
    }

    public void Activate(SpellCastInfo castInfo)
    {
        foreach (Targeter targeter in targeters)
        {
            targeter.Activate(castInfo);
        }
    }

    public SpellCastInfo GetCastInfoPrototype(IAffectable caster)
    {
        return new SpellCastInfo(caster, CastInfo);
    }

    public void ParseXML<T>(XMLNode spell) where T : Targeter, new()
    {
        range = spell.SelectInt("range", int.MaxValue);
        cost = spell.SelectInt("cost", 0);
        Icon = spell.SelectString("icon", "");

        // If no targeter specified, assume T
        T self = new T();
        self.ParseXML(spell);
        targeters.Add(self);

        foreach (XMLNode targeter in spell.SelectList("targeter"))
        {
            string targeterType = targeter.SelectString("type");
            Targeter t = BigBoss.Types.Instantiate<Targeter>(targeterType);
            t.ParseXML(targeter);
            targeters.Add(t);
        }
    }

    public void ParseXML(XMLNode spell)
    {
        ParseXML<Self>(spell);
    }

    protected void AddAspect(Targeter targeter, List<EffectInstance> effects)
    {
        if (effects.Count == 0 || targeter == null)
            return;
        targeters.Add(new Targeter() { Effects = effects });
    }
    
    public int GetHash()
    {
        int hash = 5;
        hash += range.GetHashCode() * 3;
        hash += cost.GetHashCode() * 4;
        foreach (Targeter aspect in targeters)
        {
            hash += aspect.GetHash();
        }
        return hash;
    }
}
