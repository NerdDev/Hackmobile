using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProbabilityList<T> : ProbabilityPool<T>
{
    protected int maxNum = 0;
    protected int tmpMax = 0;
    protected float largestDiv = -1;
    protected List<ProbContainer> itemList = new List<ProbContainer>();
    protected int Max { 
        get { 
            return tmpMax; 
        } 
        set {
            maxNum = value;
            tmpMax = value;
        }
    }

    public ProbabilityList(RandomGen rand)
        : base(rand)
    {
    }

    public ProbabilityList () : this(Probability.Rand)
    {
    }

    public ProbabilityList (ProbabilityList<T> rhs)
        : base(rhs)
    {
        this.maxNum = rhs.maxNum;
        this.tmpMax = rhs.maxNum;
        this.largestDiv = rhs.largestDiv;
        foreach (ProbContainer cont in rhs.itemList)
        {
            itemList.Add(new ProbContainer(cont));
        }
    }

    private bool AddInternal(ProbContainer cont)
    {
        itemList.Add(cont);
        if (cont.probDiv > largestDiv)
        { // If div is largest, recalc
            largestDiv = cont.probDiv;
            return true;
        }
        return false;
    }

    public override void Add(T item, float probDiv, bool unique)
    {
        ProbContainer cont = new ProbContainer(item, probDiv, unique);
        if (AddInternal(cont))
        { // Recalc all probnums since we have new largest div
            EvaluateProbNums();
        }
        else
        { // Scale number to largest div
            cont.SetNum(largestDiv);
            Max += cont.num;
        }
    }

    public override void ClearUnique()
    {
        foreach (ProbContainer cont in itemList)
            cont.skip = false;
        tmpMax = maxNum;
    }

    void EvaluateProbNums()
    {
        maxNum = 0;
        foreach (ProbContainer cont in itemList)
        {
            cont.SetNum(largestDiv);
            maxNum += cont.num;
        }
        Max = maxNum;
    }

    public override void ToLog(DebugManager.Logs log)
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
        int picked = rand.Next(Max);
        resultIndex = 0;
        int curNum = 0;
        foreach (ProbContainer cont in itemList)
        {
            curNum += cont.num;
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
                    picked += cont.num;
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
            tmpMax -= cont.num;
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

    protected class ProbContainer {
        public T item;
        public int num { get; protected set; }
        public bool skip { get; set; }
        public float probDiv { get; set; }
        public bool unique { get; set; }

        public ProbContainer(T item, float probDiv, bool unique)
        {
            this.item = item;
            this.probDiv = probDiv;
            this.unique = unique;
        }

        public ProbContainer(ProbContainer rhs)
        {
            this.item = rhs.item;
            this.num = rhs.num;
            this.skip = rhs.skip;
            this.probDiv = rhs.probDiv;
            this.unique = rhs.unique;
        }

        public void SetNum(float maxDivider)
        {
            num = (int)(maxDivider / probDiv);
        }
    }
}
