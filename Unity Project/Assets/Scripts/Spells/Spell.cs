using System;
using System.Collections.Generic;
using System.Linq;
using XML;

public class Spell : IXmlParsable
{
    SpellCastInfo info;
    internal int range;
    protected SpellCastInfo CastInfo
    {
        get
        {
            if (info == null)
                info = new SpellCastInfo(aspects);
            return info;
        }
    }
    internal List<SpellAspect> aspects = new List<SpellAspect>();

    public void Activate(IAffectable caster)
    {
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

    public void ParseXML(XMLNode spell)
    {
        range = spell.SelectInt("range");
        // If no targeter specified, assume self
        AddAspect(new Self(), GetEffects(spell.SelectList("effect")));

        foreach (XMLNode targeter in spell.SelectList("targeter"))
        {
            string targeterType = targeter.SelectString("type");
            AddAspect(BigBoss.Types.Instantiate<ITargeter>(targeterType), GetEffects(targeter.SelectList("effect")));
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
