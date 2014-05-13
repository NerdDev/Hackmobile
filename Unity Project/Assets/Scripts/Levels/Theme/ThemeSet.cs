using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ThemeSet : ThemeOption, IInitializable
{
    private ProbabilityPool<ThemeOption> _pool;
    public List<PrefabProbabilityContainer> Elements;

    [Serializable]
    public class PrefabProbabilityContainer
    {
        public float Multiplier = 1f;
        public ThemeOption Item;
    }

    public void Init()
    {
        _pool = ProbabilityPool<ThemeOption>.Create();
        foreach (PrefabProbabilityContainer cont in Elements)
        {
            if (cont.Item == null)
            {
                throw new ArgumentException("Prefab has to be not null");
            }
            if (cont.Multiplier <= 0)
            {
                cont.Multiplier = 1f;
            }
            _pool.Add(cont.Item, cont.Multiplier);
        }
    }

    public override Theme GetTheme(System.Random random)
    {
        return _pool.Get(random).GetTheme(random);
    }
}

