using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ThemeSet : MonoBehaviour, IThemeOption, IInitializable
{
    private ProbabilityPool<IThemeOption> _pool;
    public List<PrefabProbabilityContainer> Elements;

    [Serializable]
    public class PrefabProbabilityContainer
    {
        public float Multiplier = 1f;
        public IThemeOption Item;
    }

    public void Init()
    {
        _pool = ProbabilityPool<IThemeOption>.Create();
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

    public Theme GetTheme(System.Random random)
    {
        return _pool.Get(random).GetTheme(random);
    }
}

