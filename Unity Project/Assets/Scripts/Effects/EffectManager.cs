﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class EffectManager
{
    public static SortedDictionary<string, Type> effects = new SortedDictionary<string, Type>();

    static EffectManager()
    {
        Type type = typeof(EffectInstance);
        List<Type> types = type.GetSubclassesOf();
        foreach (Type t in types)
        {
            effects.Add(t.ToString(), t);
        }
    }
}