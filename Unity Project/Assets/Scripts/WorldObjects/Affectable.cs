using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Affectable : WorldObject, IAffectable
{
    protected AppliedEffects Effects;
    public WorldObject Self { get { return this; } }

    public Affectable()
    {
        Effects = new AppliedEffects(this);
    }

    #region IAffectable
    //public void ApplyEffect(EffectInstance effect)
    //{
    //    Effects.ApplyEffect(effect);
    //}

    public void ApplyEffect(IAffectable caster, EffectInstance effect)
    {
        Effects.ApplyEffect(caster, effect);
    }

    public void RemoveEffect(string effect)
    {
        Effects.RemoveEffect(effect);
    }

    public void RemoveEffect<T>() where T : EffectInstance
    {
        Effects.RemoveEffect<T>();
    }

    public bool RemoveAnEffect<T>() where T : EffectInstance
    {
        return Effects.RemoveAnEffect<T>();
    }

    public bool HasEffect<T>() where T : EffectInstance
    {
        return Effects.HasEffect<T>();
    }

    public bool HasEffect(string key)
    {
        return Effects.HasEffect(key);
    }
    #endregion
}
