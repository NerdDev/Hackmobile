using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public abstract class ProbabilityPool<T>  {

    public System.Random Rand;
    static protected int maxProbDiv = 100000;
    public bool Fresh { get; protected set; }

    public static ProbabilityPool<T> Create(System.Random rand = null)
    {
        return new ProbabilityList<T>(rand);
    }

    public ProbabilityPool(System.Random rand)
    {
        Rand = rand;
    }

    public ProbabilityPool() : this(Probability.Rand)
    {
    }

    public ProbabilityPool(ProbabilityList<T> rhs)
    {
        this.Rand = rhs.Rand;
    }

    public abstract void Add(T item, double multiplier, bool unique);

    public virtual void Add(T item)
    {
        if (item is ProbabilityItem)
        {
            ProbabilityItem p = (ProbabilityItem)item;
            Add(item, p.Multiplier, p.Unique);
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
        if (amount == 1)
            return new List<T>(new[] {Get()});
        List<T> picks = new List<T>();
        T item;
        for (int i = 0; i < amount; i++)
        {
            if (Get(out item))
                picks.Add(item);
        }
        return picks;
    }

    public abstract ProbabilityPool<T> Filter(Func<T, bool> filter);

    public abstract void ClearSkipped();

    public abstract void ToLog(Logs log, string name = "");

}
