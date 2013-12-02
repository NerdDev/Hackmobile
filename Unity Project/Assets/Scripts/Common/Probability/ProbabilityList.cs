using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ProbabilityList<T> : ProbabilityPool<T>
{
    protected List<ProbContainer> itemList = new List<ProbContainer>();
    protected double max;
    protected double uniqueTmpMax;
    protected double Max
    {
        get
        {
            return uniqueTmpMax;
        }
        set
        {
            uniqueTmpMax = value;
            max = value;
        }
    }

    public ProbabilityList(RandomGen rand)
        : base(rand)
    {
    }

    public ProbabilityList()
        : this(Probability.Rand)
    {
    }

    public ProbabilityList(ProbabilityList<T> rhs)
        : base(rhs)
    {
        this.Max = rhs.Max;
        foreach (ProbContainer cont in rhs.itemList)
        {
            itemList.Add(new ProbContainer(cont));
        }
    }

    protected void Add(IEnumerable<ProbContainer> conts)
    {
        foreach (ProbContainer cont in conts)
            Add(cont);
    }

    public override void Add(T item, double multiplier, bool unique)
    {
        Add(new ProbContainer(item, multiplier, unique));
    }

    protected void Add(ProbContainer cont)
    {
        if (cont.multiplier < 0)
            throw new ArgumentException("Multiplier cannot be less than zero: " + cont.multiplier);
        itemList.Add(cont);
        Max += cont.multiplier;
    }

    public override void ClearUnique()
    {
        foreach (ProbContainer cont in itemList)
            cont.skip = false;
        uniqueTmpMax = max;
    }

    public override void ToLog(Logs log)
    {
        if (BigBoss.Debug.logging(log) && BigBoss.Debug.Flag(DebugManager.DebugFlag.Probability))
        {
            BigBoss.Debug.printHeader(log, "Probability List - State");
            BigBoss.Debug.w(log, "Max Num: " + max + ", Current Max: " + uniqueTmpMax);
            foreach (ProbContainer cont in itemList)
            {
                BigBoss.Debug.w(log, cont.multiplier + " - " + cont.item);
            }
            BigBoss.Debug.printFooter(log);
        }
    }

    public bool Get(out T item, out int resultIndex)
    {
        double picked = rand.NextDouble();
        picked = picked.Modulo(Max);
        resultIndex = 0;
        double curNum = 0;
        foreach (ProbContainer cont in itemList)
        {
            curNum += cont.multiplier;
            if (picked < curNum)
            {
                if (!cont.skip)
                {
                    HandleUnique(cont);
                    item = cont.item;
                    return true;
                }
                else
                {
                    picked += cont.multiplier;
                }
            }
            resultIndex++;
        }
        item = default(T);
        return false;
    }

    protected bool HandleUnique(ProbContainer cont)
    {
        if (cont.unique)
        {
            cont.skip = true;
            uniqueTmpMax -= cont.multiplier;
            Fresh = false;
        }
        return true;
    }

    public override bool Get(out T item)
    {
        int num;
        return Get(out item, out num);
    }

    bool GetRemove(out T item)
    {
        int index;
        if (Get(out item, out index))
        {
            itemList.RemoveAt(index);
            return true;
        }
        return false;
    }

    public override bool GetUnique(out T item)
    {
        int index;
        bool ret = Get(out item, out index);
        itemList[index].skip = true;
        return ret;
    }

    public override ProbabilityPool<T> Filter(System.Func<T, bool> filter)
    {
        ProbabilityList<T> ret = new ProbabilityList<T>(rand);
        List<ProbContainer> filtered = new List<ProbContainer>();
        foreach (ProbContainer cont in itemList)
        {
            if (filter(cont.item))
                filtered.Add(cont);
        }
        ret.Add(filtered);
        return ret;
    }

    protected class ProbContainer
    {
        public T item;
        public bool skip { get; set; }
        public double multiplier { get; set; }
        public bool unique { get; set; }

        public ProbContainer(T item, double multiplier, bool unique)
        {
            this.item = item;
            this.multiplier = multiplier;
            this.unique = unique;
        }

        public ProbContainer(ProbContainer rhs)
        {
            this.item = rhs.item;
            this.skip = rhs.skip;
            this.multiplier = rhs.multiplier;
            this.unique = rhs.unique;
        }
    }
}
