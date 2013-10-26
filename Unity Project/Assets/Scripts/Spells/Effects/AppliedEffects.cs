using System;
using System.Collections.Generic;
using XML;

public class AppliedEffects : SortedDictionary<string, EffectInstance>, Field, IAffectable
{
    IAffectable owner;
    WorldObject IAffectable.Self { get { return owner.Self; } }

    public AppliedEffects(IAffectable owner)
    {
        this.owner = owner;
    }

    public void ParseXML(XMLNode x, string name)
    {
    }

    public virtual void ApplyEffect(EffectInstance effect)
    {
        if (effect.turnsToProcess != 0)
        {
            if (ContainsKey(effect.effect))
            {
                this[effect.effect] = this[effect.effect].Merge(effect);
            }
            else
            {
                this.Add(effect.effect, effect.ActivateOnObject(owner));
            }
        }
        else
        {
            effect.ActivateOnObject(owner);
        }
    }

    public void RemoveEffect(string effect)
    {
        if (this.ContainsKey(effect))
        {
            BigBoss.Time.RemoveFromUpdateList(this[effect]);
            this[effect].IsActive = false;
            this.Remove(effect);
        }
        else
        {
            //effect doesn't exist on the NPC
        }
    }

    public void RemoveEffect<T>() where T : EffectInstance
    {
        Type t = typeof(T);
        RemoveEffect(t.ToString());
    }

    public bool RemoveAnEffect<T>() where T : EffectInstance
    {
        EffectInstance inst = null;
        foreach (EffectInstance instance in this.Values)
        {
            if (instance is T)
            {
                inst = instance;
            }
        }
        if (inst != null)
        {
            this.Remove(inst.ToString());
            return true;
        }
        return false;
    }

    public bool HasEffect<T>() where T : EffectInstance
    {
        foreach (EffectInstance instance in this.Values)
        {
            if (instance is T)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasEffect(string key)
    {
        if (this.ContainsKey(key))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetDefault()
    {
    }
}
