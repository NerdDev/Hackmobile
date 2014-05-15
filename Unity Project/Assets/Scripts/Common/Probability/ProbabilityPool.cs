using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public abstract class ProbabilityPool<T> : IEnumerable<ProbabilityItem<T>>, IEnsureType
{
    public const double MAX_MULTIPLIER = 100000;
    public const double MIN_MULTIPLIER = .0000000001;
    public bool Fresh { get; protected set; }
    public abstract int Count { get; }

    public static ProbabilityPool<T> Create()
    {
        return new ProbabilityList<T>();
    }

    public ProbabilityPool()
    {
    }

    public abstract void Add(T item, double multiplier, bool unique);

    public abstract int Remove(T item, bool all);

    public abstract void AddAll(ProbabilityPool<T> rhs);

    public void Add(T item, double multiplier)
    {
        Add(item, multiplier, false);
    }

    public void Add(ProbabilityItem<T> probItem)
    {
        Add(probItem.Item, probItem.Multiplier, probItem.Unique);
    }

    public virtual void Add(T item)
    {
        if (item is IProbabilityItem)
        {
            IProbabilityItem p = (IProbabilityItem)item;
            Add(item, p.Multiplier, p.Unique);
        }
        else
        {
            Add(item, 1, false);
        }
    }

    public abstract bool Take(System.Random random, out T item);

    public abstract bool Get(System.Random random, out T item);

    public T Get(System.Random random)
    {
        T item;
        Get(random, out item);
        return item;
    }

    public List<T> Get(System.Random random, int amount)
    {
        T item;
        if (amount == 1)
        {
            List<T> ret = new List<T>(1);
            if (Get(random, out item))
            {
                ret.Add(item);
            }
            return ret;
        }
        List<T> picks = new List<T>();
        for (int i = 0; i < amount; i++)
        {
            if (Get(random, out item))
                picks.Add(item);
        }
        return picks;
    }

    public List<T> Get(System.Random random, int min, int max)
    {
        int num = random.Next(min, max + 1);
        if (num == 0) return new List<T>();

        return Get(random, num);
    }

    public abstract ProbabilityPool<T> Filter(Func<T, bool> filter);

    public abstract void Freshen();

    public abstract void BeginTaking();

    public abstract void EndTaking();

    public abstract void ToLog(Log log, string name = "");

    public void EnsureType(Type target)
    {
        foreach (var item in this)
        {
            this.EnsureType(target, item);
        }
    }

    public abstract IEnumerable<ProbabilityChance<T>> GetChances();

    public abstract IEnumerator<ProbabilityItem<T>> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
