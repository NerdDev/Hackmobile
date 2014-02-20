using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public abstract class ProbabilityPool<T>  
{
    static protected int maxProbDiv = 100000;
    public bool Fresh { get; protected set; }

    public static ProbabilityPool<T> Create()
    {
        return new ProbabilityList<T>();
    }

    public ProbabilityPool()
    {
    }

    public abstract void Add(T item, double multiplier, bool unique);

    public void Add(T item, double multiplier)
    {
        Add(item, multiplier, false);
    }

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

    public abstract bool Get(System.Random random, out T item);

    public T Get(System.Random random)
    {
        T item;
        Get(random, out item);
        return item;
    }

    public List<T> Get(System.Random random, int amount)
    {
        if (amount == 1)
            return new List<T>(new[] {Get(random)});
        List<T> picks = new List<T>();
        T item;
        for (int i = 0; i < amount; i++)
        {
            if (Get(random, out item))
                picks.Add(item);
        }
        return picks;
    }

    public abstract ProbabilityPool<T> Filter(Func<T, bool> filter);

    public abstract void ClearSkipped();

    public abstract void ToLog(Logs log, string name = "");

}
