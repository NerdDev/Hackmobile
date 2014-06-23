using System;
using System.Collections.Generic;

public class AppliedEffects : IXmlParsable, IAffectable
{
    public Dictionary<string, EffectInstance> StoredMergedEffects = new Dictionary<string, EffectInstance>();
    public Dictionary<int, EffectInstance> StoredStackingEffects = new Dictionary<int, EffectInstance>();

    [Copyable]
    IAffectable owner;
    WorldObject IAffectable.Self { get { return owner.Self; } }

    AppliedEffects() { } //used for copy extension

    public AppliedEffects(IAffectable owner)
    {
        this.owner = owner;
    }

    public void ParseXML(XMLNode x)
    {
    }

    public virtual void ApplyEffect(IAffectable caster, EffectInstance effect)
    {
        EffectInstance inst = effect.NewInstance(caster, owner);
        if (effect.TurnsToProcess != 0)
        {
            if (effect.EffectType == EffectInstance.StackType.Merging)
            {
                ApplyMergeEffect(caster, inst);
            }
            else
            {
                ApplyStackingEffect(caster, inst);
            }
        }
        else
        {
            inst.initialize();
        }
    }

    private void ApplyMergeEffect(IAffectable caster, EffectInstance effect)
    {
        if (StoredMergedEffects.ContainsKey(effect.Name))
        {
            StoredMergedEffects[effect.Name] = StoredMergedEffects[effect.Name].Merge(effect);
        }
        else
        {
            StoredMergedEffects.Add(effect.Name, effect);
            effect.initialize();
        }
    }

    private void ApplyStackingEffect(IAffectable caster, EffectInstance effect)
    {
        if (StoredStackingEffects.ContainsKey(effect.ID))
        {
            StoredStackingEffects[effect.ID] = StoredStackingEffects[effect.ID].Merge(effect);
        }
        else
        {
            StoredStackingEffects.Add(effect.ID, effect);
            effect.initialize();
        }
    }

    public bool RemoveEffect(string effect)
    {
        if (StoredMergedEffects.ContainsKey(effect))
        {
            BigBoss.Time.Remove(StoredMergedEffects[effect]);
            StoredMergedEffects.Remove(effect);
            return true;
        }
        else
        {
            return false;
            //effect doesn't exist on the NPC
        }
    }

    public bool RemoveEffect(int id)
    {
        if (StoredStackingEffects.ContainsKey(id))
        {
            BigBoss.Time.Remove(StoredStackingEffects[id]);
            StoredStackingEffects.Remove(id);
            return true;
        }
        else
        {
            //effect doesn't exist on the NPC
            return false;
        }
    }

    public bool RemoveEffect(EffectInstance instance)
    {
        if (instance.EffectType == EffectInstance.StackType.Merging)
        {
            return RemoveEffect(instance.Name);
        }
        else if (instance.EffectType == EffectInstance.StackType.Stacking)
        {
            return RemoveEffect(instance.ID);
        }
        return false;
    }

    public bool RemoveEffect<T>() where T : EffectInstance
    {
        Type t = typeof(T);
        if (!RemoveEffect(t.ToString()))
        {
            EffectInstance inst = null;
            foreach (EffectInstance instance in StoredStackingEffects.Values)
            {
                if (instance is T)
                {
                    inst = instance;
                }
            }
            if (inst != null)
            {
                return this.RemoveEffect(inst.ID);
            }
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool RemoveAnEffect<T>() where T : EffectInstance
    {
        EffectInstance inst = null;
        foreach (EffectInstance instance in StoredMergedEffects.Values)
        {
            if (instance is T)
            {
                inst = instance;
            }
        }
        if (inst != null)
        {
            this.RemoveEffect(inst.ToString());
            return true;
        }
        foreach (EffectInstance instance in StoredStackingEffects.Values)
        {
            if (instance is T)
            {
                inst = instance;
            }
        }
        if (inst != null)
        {
            this.RemoveEffect(inst.ID);
            return true;
        }
        return false;
    }

    public bool HasEffect(EffectInstance inst)
    {
        foreach (EffectInstance instance in StoredMergedEffects.Values)
        {
            if (instance.Equals(inst))
            {
                return true;
            }
        }
        foreach (EffectInstance instance in StoredStackingEffects.Values)
        {
            if (instance.Equals(inst))
            {
                return true;
            }
        }
        return false;
    }

    public bool HasEffect<T>() where T : EffectInstance
    {
        foreach (EffectInstance instance in StoredMergedEffects.Values)
        {
            if (instance is T)
            {
                return true;
            }
        }
        foreach (EffectInstance instance in StoredStackingEffects.Values)
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
        if (StoredMergedEffects.ContainsKey(key))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HasEffect(int key)
    {
        if (StoredStackingEffects.ContainsKey(key))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
