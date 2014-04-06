using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class ThemeElementBundle : IInitializable, IThemeElementBundle
{
    private ProbabilityPool<SmartThemeElement> _pool;
    public List<PrefabProbabilityContainer> Elements;
    public SmartThemeElement SmartElement { get; set; }

    [Serializable]
    public class PrefabProbabilityContainer
    {
        public float Multiplier = 1f;
        public SmartThemeElement Item;
    }

    public void Init()
    {
        _pool = ProbabilityPool<SmartThemeElement>.Create();
        foreach (PrefabProbabilityContainer cont in Elements)
        {
            if (cont.Item == null)
            {
                throw new ArgumentException("Prefab has to be not null");
            }
            cont.Item.Init();
            if (cont.Multiplier <= 0)
            {
                cont.Multiplier = 1f;
            }
            _pool.Add(cont.Item, cont.Multiplier);
        }
    }

    public SmartThemeElement Select(System.Random rand)
    {
        SmartElement = _pool.Get(rand);
        return SmartElement;
    }

    public void EnsureType(Type target)
    {
        foreach (PrefabProbabilityContainer cont in Elements)
        {
            cont.Item.EnsureType(target);
        }
    }
}

public interface IThemeElementBundle
{
    SmartThemeElement SmartElement { get; }
}
