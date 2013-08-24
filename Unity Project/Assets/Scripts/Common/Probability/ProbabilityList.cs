using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProbabilityList<T> where T : ProbabilityItem
{
    protected RandomGen rand;
    protected int maxNum = 0;
    protected int largestDiv = -1;
    protected List<ProbContainer> itemList = new List<ProbContainer>();
    protected bool hasUnique = false;

    public ProbabilityList(RandomGen rand)
    {
        this.rand = rand;
    }

    public ProbabilityList () : this(Probability.Rand)
    {
    }

    public ProbabilityList (ProbabilityList<T> rhs)
    {
        this.rand = rhs.rand;
        this.maxNum = rhs.maxNum;
        this.largestDiv = rhs.largestDiv;
        foreach (ProbContainer cont in rhs.itemList)
        {
            itemList.Add(new ProbContainer(cont));
        }
    }

    private bool AddInternal(ProbContainer cont)
    {
        itemList.Add(cont);
        if (cont.item.IsUnique())
            hasUnique = true;
        int probDiv = cont.item.ProbabilityDiv();
        if (probDiv > largestDiv)
        { // If div is largest, recalc
            largestDiv = probDiv;
            return true;
        }
        return false;
    }

    public void Add(T item)
    {
        ProbContainer cont = new ProbContainer(item);
        if (AddInternal(cont))
        { // Recalc all probnums since we have new largest div
            evaluateProbNums();
        }
        else
        { // Scale number to largest div
            ProbContainer lastCont = itemList[itemList.Count - 2];
            cont.SetNum(largestDiv, lastCont.num);
            maxNum = cont.num;
        }
    }

    public void Add(List<T> items)
    {
        foreach (T item in items)
        {
            AddInternal(new ProbContainer(item));
        }
        evaluateProbNums();
    }

    void evaluateProbNums()
    {
        maxNum = 0;
        foreach (ProbContainer cont in itemList)
        {
            cont.SetNum(largestDiv, maxNum);
            maxNum = cont.num;
        }
    }

    public void ToLog(DebugManager.Logs log)
    {
        if (DebugManager.logging(log) && DebugManager.Flag(DebugManager.DebugFlag.Probability))
        {
            DebugManager.printHeader(log, "Probability List - State");
            DebugManager.w(log, "Largest Div: " + largestDiv + ", Max Num: " + maxNum);
            foreach (ProbContainer cont in itemList)
            {
                DebugManager.w(log, cont.num + " - " + cont.item);
            }
            DebugManager.printFooter(log);
        }
    }

    public bool Get(out T item, out int resultIndex)
    {
        int picked = rand.Next(maxNum);
        resultIndex = 0;
        foreach (ProbContainer cont in itemList)
        {
            if (picked < cont.num)
            {
                item = cont.item;
                return true;
            }
            resultIndex++;
        }
        item = default(T);
        return false;
    }

    public bool Get(out T item)
    {
        int picked = rand.Next(maxNum);
        foreach (ProbContainer cont in itemList)
        {
            if (picked < cont.num)
            {
                item = cont.item;
                return true;
            }
        }
        item = default(T);
        return false;
    }

    public T Get()
    {
        T item;
        Get(out item);
        return item;
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

    // Gets desired rolls from the list
    // While only walking it once
    List<T> Get(List<int> randNums)
    {
        randNums.Sort();
        List<T> ret = new List<T>();
        int removeCount = -1;
        foreach (ProbContainer cont in itemList)
        {
            foreach (int picked in randNums)
            {
                if (picked < cont.num)
                {
                    ret.Add(cont.item);
                    removeCount++;
                }
                else
                { // Since randNums are sorted we can skip rest
                    continue;
                }
            }
            if (removeCount > 0)
            {
                randNums.RemoveRange(0, removeCount);
                removeCount = 0;
            }
        }
        return ret;
    }

    public List<T> GetUnique(int amount)
    {
        List<T> ret = new List<T>();
        ProbabilityList<T> copy = new ProbabilityList<T>(this);
        T item;
        for (int i = 0; i < amount; i++)
        {
            if (copy.GetRemove(out item))
            {
                ret.Add(item);
            }
        }
        return ret;
    }

    public List<T> Get(int amount)
    {
        if (hasUnique)
        { // Has a unique item on the list
            List<T> ret = new List<T>();
            ProbabilityList<T> copy = new ProbabilityList<T>(this);
            int index;
            T item;
            for (int i = 0; i < amount; i++)
            {
                if (copy.Get(out item, out index))
                {
                    if (item.IsUnique())
                    {
                        copy.itemList.RemoveAt(index);
                    }
                    ret.Add(item);
                }
            }
            return ret;
        }
        else
        { // Simpler if no uniques
            List<int> randNums = new List<int>();
            for (int i = 0; i < amount; i++)
            {
                randNums.Add(rand.Next(maxNum));
            }
            return Get(randNums);
        }
    }

    protected class ProbContainer {
        public T item;
        private int num_ = 0;
        public int num { get { return num_; } }

        public ProbContainer(T item)
        {
            this.item = item;
        }

        public ProbContainer(ProbContainer rhs)
        {
            this.item = rhs.item;
            this.num_ = rhs.num_;
        }

        public void SetNum(int maxDivider, int curNum)
        {
            float ratio = ((float)item.ProbabilityDiv()) / ((float)maxDivider);
            num_ = (int)(1F / ratio);
            num_ += curNum;
        }
    }
}
