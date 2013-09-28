using System;
using System.Collections.Generic;

public class LeveledPool<T> : ProbabilityPool<T>
{
    HashSet<ProbContainer> prototypePool = new HashSet<ProbContainer>();
    Func<int, float> levelCurve;
    ProbabilityPool<T> currentPool;
    int curLevel = -1;

    public LeveledPool(RandomGen rand, Func<int, float> curve)
        : base(rand)
    {
        this.levelCurve = curve;
    }

    public LeveledPool(Func<int, float> curve)
        : this(Probability.SpawnRand, curve)
    {
    }

    #region Add
    public void Add(T item, float probDiv, bool unique, int level)
    {
        Add(new ProbContainer(item, probDiv, unique, level));
    }

    protected void Clear()
    {
        if (curLevel != -1)
        {
            currentPool = ProbabilityPool<T>.Create(rand);
            curLevel = -1;
        }
    }

    public void Add(T item)
    {
        if (item is ProbabilityLevItem)
        {
            ProbabilityLevItem p = (ProbabilityLevItem)item;
            Add(item, p.ProbabilityDiv(), p.IsUnique(), p.GetLevel());
        }
        base.Add(item);
    }

    public override void Add(T item, float probDiv, bool unique)
    {
        Add(item, probDiv, unique, 1);
    }

    protected void Add(ProbContainer cont)
    {
        prototypePool.Add(cont);
        Clear();
    }
    #endregion

    protected void SetFor(int level)
    {
        if (curLevel == level)
            return;
        currentPool = ProbabilityPool<T>.Create(rand);
        foreach (ProbContainer prototype in prototypePool)
        {
            float multiplier = levelCurve(prototype.Level);
            int probDiv = (int)(prototype.ProbDiv / multiplier);
            if (probDiv < maxProbDiv)
                currentPool.Add(prototype.Item, probDiv, prototype.Unique);
        }
        curLevel = level;
    }

    #region Get
    public override bool Get(out T item)
    {
        return Get(out item, BigBoss.Player.Level);
    }

    public bool Get(out T item, int level)
    {
        SetFor(level);
        return currentPool.Get(out item);
    }

    public T Get(int level)
    {
        T item;
        Get(out item, level);
        return item;
    }

    public List<T> Get(int amount, int level)
    {
        SetFor(level);
        List<T> picks = new List<T>();
        T item;
        for (int i = 0; i < amount; i++)
        {
            if (currentPool.Get(out item))
                picks.Add(item);
        }
        return picks;
    }

    public override bool GetUnique(out T item)
    {
        return GetUnique(out item, BigBoss.Player.Level);
    }

    public List<T> GetUnique(int amount, int level)
    {
        SetFor(level);
        T item;
        List<T> ret = new List<T>();
        for (int i = 0; i < amount; i++)
        {
            if (currentPool.GetUnique(out item))
                ret.Add(item);
        }
        return ret;
    }

    public bool GetUnique(out T item, int level)
    {
        SetFor(level);
        return currentPool.GetUnique(out item);
    }
    #endregion

    public override ProbabilityPool<T> Filter(Func<T, bool> filter)
    {
        LeveledPool<T> ret = new LeveledPool<T>(rand, levelCurve);
        foreach (ProbContainer cont in prototypePool)
        {
            if (filter(cont.Item))
                ret.Add(cont);
        }
        return ret;
    }

    public override void ClearUnique()
    {
        throw new NotImplementedException();
    }

    public override void ToLog(DebugManager.Logs log)
    {
        throw new NotImplementedException();
    }

    #region Shell
    protected class ProbContainer
    {
        public T Item;
        public float ProbDiv;
        public bool Unique;
        public int Level;

        public ProbContainer(T item, float probDiv, bool unique, int level)
        {
            this.Item = item;
            this.ProbDiv = probDiv;
            this.Unique = unique;
            this.Level = level;
        }
    }
    #endregion
}
