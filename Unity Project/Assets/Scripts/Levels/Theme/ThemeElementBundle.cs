using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class ThemeElementBundle : IInitializable, IThemeElementBundle
{
    private ProbabilityPool<SmartThemeElement> _pool;
    private MultiMap<ProbabilityPool<SmartThemeElement>> _map;
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
        _map = new MultiMap<ProbabilityPool<SmartThemeElement>>();
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
            ProbabilityPool<SmartThemeElement> pool;
            if (!_map.TryGetValue(cont.Item.GridWidth, cont.Item.GridLength, out pool))
            {
                pool = ProbabilityPool<SmartThemeElement>.Create();
                _map[cont.Item.GridWidth, cont.Item.GridLength] = pool;
            }
            pool.Add(cont.Item);
        }
    }

    public SmartThemeElement Select(System.Random rand)
    {
        SmartElement = _pool.Get(rand);
        return SmartElement;
    }

    public bool Select(System.Random rand, int width, int length, out SmartThemeElement element)
    {
        ProbabilityPool<SmartThemeElement> pool;
        if (_map.TryGetValue(width, length, out pool))
        {
            element = pool.Get(rand);
            SmartElement = element;
            return true;
        }
        element = null;
        return false;
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
