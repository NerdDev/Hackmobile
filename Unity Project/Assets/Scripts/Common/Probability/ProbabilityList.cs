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

    public ProbabilityList(System.Random rand)
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

    public override void Add(T item, double multiplier, bool unique = false)
    {
        Add(new ProbContainer(item, multiplier, unique));
    }

    protected void Add(ProbContainer cont)
    {
        if (cont.Multiplier < 0)
            throw new ArgumentException("Multiplier cannot be less than zero: " + cont.Multiplier);
        itemList.Add(cont);
        Max += cont.Multiplier;
    }

    public override void ClearSkipped()
    {
        foreach (ProbContainer cont in itemList)
            cont.Skip = false;
        uniqueTmpMax = max;
        Fresh = true;
    }

    public override void ToLog(Logs log, string name = "")
    {
        if (BigBoss.Debug.logging(log) && BigBoss.Debug.Flag(DebugManager.DebugFlag.Probability))
        {
            BigBoss.Debug.printHeader(log, "Probability List - " + name);
            BigBoss.Debug.w(log, "Max Num: " + max + ", Current Max: " + uniqueTmpMax);
            BigBoss.Debug.w(log, "Percent - Alotted - Item");
            foreach (ProbContainer cont in itemList)
            {
                BigBoss.Debug.w(log, (cont.Multiplier / max * 100) + "% - " + cont.Multiplier + " - " + cont.Multiplier + " - " + cont.Item);
            }
            BigBoss.Debug.printFooter(log);
        }
    }

    public bool Get(out T item, out int resultIndex)
    {
        double picked = Rand.NextDouble() * Max;
        resultIndex = 0;
        double curNum = 0;
        foreach (ProbContainer cont in itemList)
        {
            if (cont.Skip) continue;
            curNum += cont.Multiplier;
            if (picked < curNum)
            {
                if (cont.Unique)
                    SetToSkip(cont);
                item = cont.Item;
                return true;
            }
            resultIndex++;
        }
        item = default(T);
        return false;
    }

    protected void SetToSkip(ProbContainer cont)
    {
        cont.Skip = true;
        uniqueTmpMax -= cont.Multiplier;
        Fresh = false;
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

    public override ProbabilityPool<T> Filter(System.Func<T, bool> filter)
    {
        ProbabilityList<T> ret = new ProbabilityList<T>(Rand);
        List<ProbContainer> filtered = new List<ProbContainer>();
        foreach (ProbContainer cont in itemList)
        {
            if (filter(cont.Item))
                filtered.Add(cont);
        }
        ret.Add(filtered);
        return ret;
    }

    protected class ProbContainer
    {
        public T Item;
        public bool Skip;
        public double Multiplier;
        public bool Unique;

        public ProbContainer(T item, double multiplier, bool unique)
        {
            this.Item = item;
            this.Multiplier = multiplier;
            this.Unique = unique;
        }

        public ProbContainer(ProbContainer rhs)
        {
            this.Item = rhs.Item;
            this.Skip = rhs.Skip;
            this.Multiplier = rhs.Multiplier;
            this.Unique = rhs.Unique;
        }
    }
}
