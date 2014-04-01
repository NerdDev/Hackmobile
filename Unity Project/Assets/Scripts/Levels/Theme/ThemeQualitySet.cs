using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class ThemeQualitySet : ProbabilityPool<ThemeElement>, IInitializable
{
    private ProbabilityPool<ThemeElement> _pool;
    public List<PrefabProbabilityContainer> Elements;

    [Serializable]
    public class PrefabProbabilityContainer
    {
        public float Multiplier = 1f;
        public bool Unique;
        public ThemeElement Item;
    }

    public void Init()
    {
        _pool = ProbabilityPool<ThemeElement>.Create();
        foreach (PrefabProbabilityContainer cont in Elements)
        {
            if (cont.Item == null)
            {
                throw new ArgumentException("Prefab has to be not null");
            }
            _pool.Add(cont.Item, cont.Multiplier, cont.Unique);
        }
    }

    #region Probability Pool
    public override int Count
    {
        get { return _pool.Count; }
    }

    public override void Add(ThemeElement item, double multiplier, bool unique)
    {
        _pool.Add(item, multiplier, unique);
    }

    public override int Remove(ThemeElement item, bool all)
    {
        return _pool.Remove(item, all);
    }

    public override void AddAll(ProbabilityPool<ThemeElement> rhs)
    {
        _pool.AddAll(rhs);
    }

    public override bool Get(System.Random random, out ThemeElement item)
    {
        return _pool.Get(random, out item);
    }

    public override ProbabilityPool<ThemeElement> Filter(Func<ThemeElement, bool> filter)
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

    public override IEnumerator<ProbabilityItem<ThemeElement>> GetEnumerator()
    {
        return _pool.GetEnumerator();
    }
    #endregion
}