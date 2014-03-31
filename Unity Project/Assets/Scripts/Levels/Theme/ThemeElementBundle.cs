using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class ThemeElementBundle : MonoBehaviour
{
    public ThemeElementQualitySet Normal;
    public ThemeElementQualitySet Destroyed;
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
}

[Serializable]
public class ThemeElementQualitySet : PrefabProbabilityPool<ThemeElement>
{
}
