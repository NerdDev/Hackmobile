using System;
using System.Linq;
using System.Collections.Generic;

public delegate double LeveledCurve(ushort gravityLevel, ushort entryLevel);
public class LeveledPool<T> : ProbabilityPool<T>
{
    HashSet<ProbContainer> prototypePool = new HashSet<ProbContainer>();
    internal LeveledCurve levelCurve;
    ProbabilityPool<T> currentPool;
    int curLevel = -1;
    public override int Count
    {
        get
        {
            if (curLevel == -1) return 0;
            return currentPool.Count;
        }
    }

    public LeveledPool()
    {

    }

    public LeveledPool(LeveledCurve curve)
    {
        this.levelCurve = curve;
    }

    #region Add
    public void Add(T item, double multiplier, bool unique, ushort level)
    {
        Clear();
        AddInternal(item, multiplier, unique, level);
    }

    private void AddInternal(T item, double multiplier, bool unique, ushort level)
    {
        Add(new ProbContainer(item, multiplier, unique, level));
    }

    protected void Clear()
    {
        if (curLevel != -1)
        {
            currentPool = ProbabilityPool<T>.Create();
            curLevel = -1;
        }
    }

    public override void Add(T item)
    {
        if (item is IProbabilityLevItem)
        {
            IProbabilityLevItem p = (IProbabilityLevItem)item;
            Add(item, p.Multiplier, p.Unique, p.Level);
        }
        else
        {
            base.Add(item);
        }
    }

    public override int Remove(T item, bool all)
    {
        Clear();
        List<ProbContainer> toRemove = new List<ProbContainer>();
        foreach (var v in prototypePool)
        {
            if (item.Equals(v.Item))
            {
                toRemove.Add(v);
                if (!all) break;
            }
        }
        foreach (var p in toRemove)
        {
            prototypePool.Remove(p);
        }
        return toRemove.Count;
    }

    public override void Add(T item, double multiplier, bool unique)
    {
        Add(item, multiplier, unique, 1);
    }

    protected void Add(ProbContainer cont)
    {
        prototypePool.Add(cont);
        Clear();
    }

    public override void AddAll(ProbabilityPool<T> rhs)
    {
        LeveledPool<T> lrhs = rhs as LeveledPool<T>;
        if (lrhs == null)
        {
            foreach (ProbabilityItem<T> p in rhs)
            {
                AddInternal(p.Item, p.Multiplier, p.Unique, 1);
            }
        }
        else
        {
            foreach (ProbabilityItem<T> p in rhs)
            {
                ProbContainer cont = (ProbContainer)p;
                AddInternal(cont.Item, cont.Multiplier, cont.Unique, cont.Level);
            }
        }
        Clear();
    }
    #endregion

    public void SetFor(ushort level)
    {
        if (curLevel == level)
            return;
        currentPool = ProbabilityPool<T>.Create();
        foreach (ProbContainer prototype in prototypePool)
        {
            double multiplier = levelCurve(level, prototype.Level);
            multiplier = prototype.Multiplier * multiplier;
            if (multiplier < MAX_MULTIPLIER && multiplier > MIN_MULTIPLIER)
                currentPool.Add(prototype.Item, multiplier, prototype.Unique);
        }
        curLevel = level;
    }

    #region Get
    public override bool Get(System.Random random, out T item)
    {
        if (curLevel < 0)
        {
            item = default(T);
            return false;
        }
        return Get(random, out item, (ushort)curLevel);
    }

    public bool Get(System.Random random, out T item, ushort level)
    {
        SetFor(level);
        return currentPool.Get(random, out item);
    }

    public virtual List<T> Get(System.Random random, int amount, ushort level)
    {
        SetFor(level);
        List<T> picks = currentPool.Get(random, amount);
        return picks;
    }

    public virtual List<T> Get(System.Random random, int min, int max, ushort level)
    {
        SetFor(level);
        List<T> picks = currentPool.Get(random, min, max);
        return picks;
    }

    public override bool Take(Random random, out T item)
    {
        return currentPool.Take(random, out item);
    }

    public bool Take(Random random, out T item, ushort level)
    {
        SetFor(level);
        return Take(random, out item);
    }
    #endregion

    public override ProbabilityPool<T> Filter(Func<T, bool> filter)
    {
        LeveledPool<T> ret = new LeveledPool<T>(levelCurve);
        foreach (ProbContainer cont in prototypePool)
        {
            if (filter(cont.Item))
                ret.Add(cont);
        }
        return ret;
    }

    public override void Freshen()
    {
        if (curLevel != -1)
        {
            currentPool.Freshen();
        }
    }

    public override void ToLog(Log log, string name = "")
    {
        if (BigBoss.Debug.logging(log))
        {
            BigBoss.Debug.printHeader(log, "LeveledPool");
            BigBoss.Debug.w(log, "Item - Level - Multiplier - Unique");
            foreach (ProbContainer cont in prototypePool)
            {
                BigBoss.Debug.w(log, cont.Item + " - " + cont.Level + " - " + cont.Multiplier + " - " + cont.Unique);
            }
            if (curLevel != -1)
            {
                BigBoss.Debug.w(log, "Current level is " + curLevel);
                currentPool.ToLog(log, name);
            }
            BigBoss.Debug.printFooter(log, "LeveledPool");
        }
    }

    #region Shell
    protected class ProbContainer : ProbabilityItem<T>
    {
        public ushort Level;

        public ProbContainer(T item, double multiplier, bool unique, ushort level)
            : base(item, multiplier, unique)
        {
            this.Level = level;
        }
    }
    #endregion

    public override IEnumerator<ProbabilityItem<T>> GetEnumerator()
    {
        if (curLevel != -1)
        {
            return currentPool.GetEnumerator();
        }
        else
        {
            return prototypePool.Cast<ProbabilityItem<T>>().GetEnumerator();
        }
    }

    public override void BeginTaking()
    {
        if (curLevel != -1)
        {
            currentPool.BeginTaking();
        }
    }

    public override void EndTaking()
    {
        if (curLevel != -1)
        {
            currentPool.EndTaking();
        }
    }

    public override IEnumerable<ProbabilityChance<T>> GetChances()
    {
        if (curLevel != -1)
        {
            foreach (var v in currentPool.GetChances())
            {
                yield return v;
            }
        }
    }
}
