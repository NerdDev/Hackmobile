using System;
using System.Collections.Generic;

public interface IAffectable
{
    WorldObject Self { get; }
    //void ApplyEffect(EffectInstance effect);
    void ApplyEffect(IAffectable caster, EffectInstance effect);
    bool RemoveEffect(string effect);
    bool RemoveEffect(int id);
    bool RemoveEffect(EffectInstance inst);
    bool RemoveEffect<T>() where T : EffectInstance;
    bool RemoveAnEffect<T>() where T : EffectInstance;
    bool HasEffect<T>() where T : EffectInstance;
    bool HasEffect(string key);
    bool HasEffect(int id);
    bool HasEffect(EffectInstance inst);
}
