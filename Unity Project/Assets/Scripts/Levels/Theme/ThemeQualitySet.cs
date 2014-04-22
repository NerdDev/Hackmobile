using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class ThemeQualitySet : ProbabilityPool<ThemeElement>, IEnsureType
{
    private ProbabilityPool<ThemeElement> _pool;
    protected ProbabilityPool<ThemeElement> pool
    {
        get
        {
            if (_pool == null)
            {
                Init();
            }
            return _pool;
        }
    }
    public List<PrefabProbabilityContainer> Elements;
    protected SmartThemeElement SmartElement;
    public int GridWidth
    {
        get
        {
            if (SmartElement == null) return 1;
            return SmartElement.GridWidth;
        }
    }
    public int GridLength
    {
        get
        {
            if (SmartElement == null) return 1;
            return SmartElement.GridLength;
        }
    }
    public string PrintChar
    {
        get
        {
            if (SmartElement == null) return "?";
            return SmartElement.PrintChar;
        }
    }

    [Serializable]
    public class PrefabProbabilityContainer
    {
        public float Multiplier = 1f;
        public bool Unique;
        public ThemeElement Item;
    }

    public void Init(SmartThemeElement smart)
    {
        SmartElement = smart;
        Init();
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
            if (cont.Multiplier <= 0)
            {
                cont.Multiplier = 1f;
            }
            cont.Item.Set = this;
            _pool.Add(cont.Item, cont.Multiplier, cont.Unique);
        }
    }

    #region Probability Pool
    public override int Count
    {
        get { return pool.Count; }
    }

    public override void Add(ThemeElement item, double multiplier, bool unique)
    {
        pool.Add(item, multiplier, unique);
    }

    public override int Remove(ThemeElement item, bool all)
    {
        return pool.Remove(item, all);
    }

    public override void AddAll(ProbabilityPool<ThemeElement> rhs)
    {
        pool.AddAll(rhs);
    }

    public override bool Get(System.Random random, out ThemeElement item)
    {
        return pool.Get(random, out item);
    }

    public override ProbabilityPool<ThemeElement> Filter(Func<ThemeElement, bool> filter)
    {
        return pool.Filter(filter);
    }

    public override void Freshen()
    {
        pool.Freshen();
    }

    public override void ToLog(Log log, string name = "")
    {
        pool.ToLog(log, name);
    }

    public override IEnumerator<ProbabilityItem<ThemeElement>> GetEnumerator()
    {
        return pool.GetEnumerator();
    }

    public override bool Take(Random random, out ThemeElement item)
    {
        return pool.Take(random, out item);
    }

    public override void BeginTaking()
    {
        pool.BeginTaking();
    }

    public override void EndTaking()
    {
        pool.EndTaking();
    }
    #endregion
}