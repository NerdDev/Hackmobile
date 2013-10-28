using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Affectable : WorldObject, IAffectable
{
    protected AppliedEffects effects;
    public WorldObject Self { get { return this; } }

    public Affectable()
    {
        effects = new AppliedEffects(this);
    }

    public override void SetParams()
    {
        base.SetParams();
        map.Add("effects", effects);
    }

    #region IAffectable
    public void ApplyEffect(EffectInstance effect)
    {
        effects.ApplyEffect(effect);
    }

    public void RemoveEffect(string effect)
    {
        effects.RemoveEffect(effect);
    }

    public void RemoveEffect<T>() where T : EffectInstance
    {
        effects.RemoveEffect<T>();
    }

    public bool RemoveAnEffect<T>() where T : EffectInstance
    {
        return effects.RemoveAnEffect<T>();
    }

    public bool HasEffect<T>() where T : EffectInstance
    {
        return effects.HasEffect<T>();
    }

    public bool HasEffect(string key)
    {
        return effects.HasEffect(key);
    }
    #endregion
}
