using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

[Serializable]
public class SmartThemeElement : MonoBehaviour, IInitializable
{
    public ThemeQualitySet Normal;
    public ThemeQualitySet Destroyed;
    public ThemeElement Proto { get { return Normal.First().Item; } }

    public ThemeElement Get(System.Random random, Percent destroyed)
    {
        if (random.Percent(destroyed))
        {
            return Destroyed.Get(random);
        }
        return Get(random);
    }

    public ThemeElement Get(System.Random random)
    {
        return Normal.Get(random);
    }

    public void Init()
    {
        Normal.Init();
        Destroyed.Init();
    }
}