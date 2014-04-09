﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

[Serializable]
public class SmartThemeElement : MonoBehaviour, IInitializable, IThemeElementBundle
{
    public ThemeQualitySet Normal;
    public ThemeElement Proto { get { return Normal.First().Item; } }

    public ThemeElement Get(System.Random random)
    {
        return Normal.Get(random);
    }

    public void Init()
    {
        foreach (ThemeQualitySet set in this.FindAllDerivedObjects<ThemeQualitySet>(false))
        {
            set.Init();
        }
    }

    public void EnsureType(Type target)
    {
        foreach (ThemeQualitySet set in this.FindAllDerivedObjects<ThemeQualitySet>(false))
        {
            set.EnsureType(target);
        }
    }

    public SmartThemeElement SmartElement
    {
        get { return this; }
    }
}