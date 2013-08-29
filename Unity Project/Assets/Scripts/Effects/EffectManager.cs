using System;
using System.Collections.Generic;

public static class EffectManager
{
    public static Dictionary<string, EffectDefinition> effects = new Dictionary<string, EffectDefinition>();

    static EffectManager()
    {
        effects.Add("poison", new PoisonEffect());
        effects.Add("nutrition", new NutritionEffect());
        effects.Add("levitation", new LevitationEffect());
        effects.Add("addhealth", new AddHealth());
    }
}
