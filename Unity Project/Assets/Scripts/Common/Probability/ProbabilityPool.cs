using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public abstract class ProbabilityPool<T>  {

    protected RandomGen rand;
    static protected int maxProbDiv = 100000;
    public bool Fresh { get; protected set; }

    public static ProbabilityPool<T> Create()
    {
        return new ProbabilityList<T>();
    }

    public static ProbabilityPool<T> Create(RandomGen rand)
    {
        return new ProbabilityList<T>(rand);
    }

    public ProbabilityPool(RandomGen rand)
    {
        this.rand = rand;
    }

    public ProbabilityPool() : this(Probability.Rand)
    {
    }

    public ProbabilityPool(ProbabilityList<T> rhs)
    {
        this.rand = rhs.rand;
    }

    public abstract void Add(T item, float probDiv, bool unique);

    public virtual void Add(T item)
    {
        if (item is ProbabilityItem)
        {
            ProbabilityItem p = (ProbabilityItem)item;
            Add(item, p.ProbabilityDiv(), p.IsUnique());
        }
        else
        {
            Add(item, 1, false);
        }
    }

    public abstract bool Get(out T item);

    public T Get()
    {
        T item;
        Get(out item);
        return item;
    }

    public List<T> Get(int amount)
    {
        List<T> picks = new List<T>();
        T item;
        for (int i = 0; i < amount; i++)
        {
            if (Get(out item))
                picks.Add(item);
        }
        return picks;
    }

    public abstract bool GetUnique(out T item);

    public List<T> GetUnique(int amount)
    {
        T item;
        List<T> ret = new List<T>();
        for (int i = 0; i < amount; i++)
        {
            if (GetUnique(out item))
                ret.Add(item);
        }
        return ret;
    }

    public abstract ProbabilityPool<T> Filter(Func<T, bool> filter);

    public abstract void ClearUnique();

    public abstract void ToLog(DebugManager.Logs log);

}
