using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Affectable : WorldObject, IAffectable
{
    [Copyable]
    protected AppliedEffects Effects;
    public WorldObject Self { get { return this; } }

    public Affectable()
        : base()
    {
        Effects = new AppliedEffects(this);
    }

    #region IAffectable
    public void ApplyEffect(IAffectable caster, EffectInstance effect)
    {
        Effects.ApplyEffect(caster, effect);
    }

    public bool RemoveEffect(EffectInstance inst)
    {
        return Effects.RemoveEffect(inst);
    }

    public bool RemoveEffect(string effect)
    {
        return Effects.RemoveEffect(effect);
    }

    public bool RemoveEffect(int id)
    {
        return Effects.RemoveEffect(id);
    }

    public bool RemoveEffect<T>() where T : EffectInstance
    {
        return Effects.RemoveEffect<T>();
    }

    public bool RemoveAnEffect<T>() where T : EffectInstance
    {
        return Effects.RemoveAnEffect<T>();
    }

    public bool HasEffect<T>() where T : EffectInstance
    {
        return Effects.HasEffect<T>();
    }

    public bool HasEffect(EffectInstance inst)
    {
        return Effects.HasEffect(inst);
    }

    public bool HasEffect(string key)
    {
        return Effects.HasEffect(key);
    }

    public bool HasEffect(int id)
    {
        return Effects.HasEffect(id);
    }
    #endregion
}
