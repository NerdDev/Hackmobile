using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class ThemeElementBundle : ProbabilityPool<SmartThemeElement>, IInitializable
{
    private ProbabilityPool<SmartThemeElement> _pool;
    public List<PrefabProbabilityContainer> Elements;

    [Serializable]
    public class PrefabProbabilityContainer
    {
        public float Multiplier = 1f;
        public bool Unique;
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
            _pool.Add(cont.Item, cont.Multiplier, cont.Unique);
        }
    }

    #region Probability Pool
    public override int Count
    {
        get { return _pool.Count; }
    }

    public override void Add(SmartThemeElement item, double multiplier, bool unique)
    {
        _pool.Add(item, multiplier, unique);
    }

    public override int Remove(SmartThemeElement item, bool all)
    {
        return _pool.Remove(item, all);
    }

    public override void AddAll(ProbabilityPool<SmartThemeElement> rhs)
    {
        _pool.AddAll(rhs);
    }

    public override bool Get(System.Random random, out SmartThemeElement item)
    {
        return _pool.Get(random, out item);
    }

    public override ProbabilityPool<SmartThemeElement> Filter(Func<SmartThemeElement, bool> filter)
    {
        return _pool.Filter(filter);
    }

    public override void Freshen()
    {
        _pool.Freshen();
    }

    public override void ToLog(Logs log, string name = "")
    {
        _pool.ToLog(log, name);
    }

    public override IEnumerator<ProbabilityItem<SmartThemeElement>> GetEnumerator()
    {
        return _pool.GetEnumerator();
    }
    #endregion
}
