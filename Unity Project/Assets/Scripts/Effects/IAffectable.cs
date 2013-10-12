using System;
using System.Collections.Generic;

public interface IAffectable
{
    void ApplyEffect(EffectInstance effect);
    void RemoveEffect(string effect);
    void RemoveEffect<T>() where T : EffectInstance;
    bool RemoveAnEffect<T>() where T : EffectInstance;
    bool HasEffect<T>() where T : EffectInstance;
    bool HasEffect(string key);
}
