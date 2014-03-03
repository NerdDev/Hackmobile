using System;
using System.Collections.Generic;

public delegate double LeveledCurve(ushort gravityLevel, ushort entryLevel);
public class LeveledPool<T> : ProbabilityPool<T>
{
    HashSet<ProbContainer> prototypePool = new HashSet<ProbContainer>();
    internal LeveledCurve levelCurve;
    ProbabilityPool<T> currentPool;
    int curLevel = -1;

	public LeveledPool() {

		}

    public LeveledPool(LeveledCurve curve)
    {
        this.levelCurve = curve;
    }

    #region Add
    public void Add(T item, double multiplier, bool unique, ushort level)
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
        if (item is ProbabilityLevItem)
        {
            ProbabilityLevItem p = (ProbabilityLevItem)item;
            Add(item, p.Multiplier, p.Unique, p.Level);
        }
        else
        {
            base.Add(item);
        }
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
    #endregion

    public void SetFor(ushort level)
    {
        if (curLevel == level)
            return;
        currentPool = ProbabilityPool<T>.Create();
        foreach (ProbContainer prototype in prototypePool)
        {
            double multiplier = levelCurve(level, prototype.Level);
            multiplier = prototype.multiplier * multiplier;
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
        return Get(random, out item, (ushort) curLevel);
    }

    public bool Get(System.Random random, out T item, ushort level)
    {
        SetFor(level);
        return currentPool.Get(random, out item);
    }

    public List<T> Get(System.Random random, int amount, ushort level)
    {
        SetFor(level);
        List<T> picks = new List<T>();
        T item;
        for (int i = 0; i < amount; i++)
        {
            if (currentPool.Get(random, out item))
                picks.Add(item);
        }
        return picks;
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

    public override void ClearSkipped()
    {
        if (curLevel != -1)
            currentPool.ClearSkipped();
    }

    public override void ToLog(Logs log, string name = "")
    {
        if (BigBoss.Debug.logging(log))
        {
            BigBoss.Debug.printHeader(log, "LeveledPool");
            BigBoss.Debug.w(log, "Item - Level - Multiplier - Unique");
            foreach (ProbContainer cont in prototypePool)
            {
                BigBoss.Debug.w(log, cont.Item + " - " + cont.Level + " - " + cont.multiplier + " - " + cont.Unique);
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
    protected class ProbContainer
    {
        public T Item;
        public double multiplier;
        public bool Unique;
        public ushort Level;

        public ProbContainer(T item, double multiplier, bool unique, ushort level)
        {
            this.Item = item;
            this.multiplier = multiplier;
            this.Unique = unique;
            this.Level = level;
        }
    }
    #endregion
}
