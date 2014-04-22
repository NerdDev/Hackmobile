using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ProbabilityList<T> : ProbabilityPool<T>
{
    private bool _takingMode = false;
    private HashSet<int> _taken;
    private int _lastTaken = -1;
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
    public override int Count
    {
        get { return itemList.Count; }
    }
    
    public ProbabilityList()
    {
    }

    public ProbabilityList(ProbabilityList<T> rhs)
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
        {
            Add(cont);
        }
    }

    public override void Add(T item, double multiplier, bool unique = false)
    {
        Add(new ProbContainer(item, multiplier, unique));
    }

    public override int Remove(T item, bool all = true)
    {
        int count = 0;
        for (int i = 0 ; i < itemList.Count ; i++)
        {
            if (item.Equals(itemList[i].Item))
            {
                itemList.RemoveAt(i);
                if (!all) return 1;
                i--;
                count++;
            }
        }
        return count;
    }

    protected void Add(ProbContainer cont)
    {
        if (cont.Multiplier <= 0) return;
        itemList.Add(cont);
        Max += cont.Multiplier;
    }

    public override void Freshen()
    {
        if (!Fresh)
        {
            foreach (ProbContainer cont in itemList)
                cont.Skip = false;
            uniqueTmpMax = max;
        }
        Fresh = true;
    }

    public override void ToLog(Log log, string name = "")
    {
        if (BigBoss.Debug.logging(log))
        {
            BigBoss.Debug.printHeader(log, "Probability List - " + name);
            BigBoss.Debug.w(log, "Max Num: " + max + ", Current Max: " + uniqueTmpMax);
            BigBoss.Debug.w(log, "Percent - Alotted - Item");
            foreach (ProbContainer cont in itemList)
            {
                double percent = cont.Multiplier / max * 100;
                if (percent < 0.000000000000000000001)
                {
                    percent = 0d;
                }
                BigBoss.Debug.w(log, percent + "% - " + cont.Multiplier + " - " + cont.Item);
            }
            BigBoss.Debug.printFooter(log, "Probability List - " + name);
        }
    }

    protected bool Get(System.Random random, out T item, out int resultIndex, bool take)
    {
        double picked = random.NextDouble() * Max;
        resultIndex = 0;
        double curNum = 0;
        foreach (ProbContainer cont in itemList)
        {
            if (!cont.Skip)
            {
                curNum += cont.Multiplier;
                if (picked < curNum)
                {
                    if (cont.Unique || take)
                    {
                        if (_takingMode)
                        {
                            if (_lastTaken != -1)
                            {
                                _taken.Add(_lastTaken);
                            }
                            _lastTaken = resultIndex;
                        }
                        SetToSkip(cont);
                    }
                    item = cont.Item;
                    return true;
                }
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

    public bool Get(System.Random random, out T item, out int resultIndex)
    {
        return Get(random, out item, out resultIndex, false);
    }

    public bool Take(System.Random random, out T item, out int resultIndex)
    {
        return Get(random, out item, out resultIndex, true);
    }

    public override bool Take(System.Random random, out T item)
    {
        int num;
        return Take(random, out item, out num);
    }

    public override bool Get(System.Random random, out T item)
    {
        int num;
        return Get(random, out item, out num);
    }

    bool GetRemove(System.Random random, out T item)
    {
        int index;
        if (Get(random, out item, out index))
        {
            itemList.RemoveAt(index);
            return true;
        }
        return false;
    }

    public override ProbabilityPool<T> Filter(System.Func<T, bool> filter)
    {
        ProbabilityList<T> ret = new ProbabilityList<T>();
        List<ProbContainer> filtered = new List<ProbContainer>();
        foreach (ProbContainer cont in itemList)
        {
            if (filter(cont.Item))
                filtered.Add(cont);
        }
        ret.Add(filtered);
        return ret;
    }

    protected class ProbContainer : ProbabilityItem<T>
    {
        public bool Skip;

        public ProbContainer(T item, double multiplier, bool unique)
            : base(item, multiplier, unique)
        {
        }

        public ProbContainer(ProbContainer rhs)
            : base(rhs.Item, rhs.Multiplier, rhs.Unique)
        {
            this.Skip = rhs.Skip;
        }
    }

    public override void AddAll(ProbabilityPool<T> rhs)
    {
        foreach (ProbabilityItem<T> p in rhs)
        {
            Add(p.Item, p.Multiplier, p.Unique);
        }
    }

    public override IEnumerator<ProbabilityItem<T>> GetEnumerator()
    {
        foreach (ProbContainer cont in itemList)
        {
            if (!cont.Skip)
            {
                yield return cont;
            }
        }
    }

    public override void BeginTaking()
    {
        _takingMode = true;
        _taken = new HashSet<int>();
    }

    public override void EndTaking()
    {
        _takingMode = false;
        foreach (int index in _taken)
        {
            itemList[index].Skip = false;
        }
        if (_lastTaken != -1)
        {
            if (!itemList[_lastTaken].Unique)
            {
                itemList[_lastTaken].Skip = false;
            }
        }
        _taken = null;
    }
}
