using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProbabilityList<T> where T : ProbabilityItem
{
    static System.Random staticRand = new System.Random();
    System.Random rand;
    int maxNum = 0;
    int largestDiv = -1;
    List<ProbContainer> itemList = new List<ProbContainer>();

    public ProbabilityList(System.Random rand)
    {
        this.rand = rand;
    }

    public ProbabilityList () : this(staticRand)
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
        int probDiv = cont.item.ProbabilityDiv();
        if (probDiv > largestDiv)
        {
            largestDiv = probDiv;
            return true;
        }
        return false;
    }

    public void Add(T item)
    {
        ProbContainer cont = new ProbContainer(item);
        if (AddInternal(cont))
        {
            evaluateProbNums();
        }
        else
        {
            cont.SetNum(largestDiv, itemList[itemList.Count - 1].num);
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
            maxNum += cont.num;
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

    public T Get()
    {
        int picked = rand.Next(maxNum);
        foreach (ProbContainer cont in itemList)
        {
            if (picked < cont.num)
            {
                return cont.item;
            }
        }
        return default(T);
    }

    T GetRemove()
    {
        int picked = rand.Next(maxNum);
        int i = 0;
        foreach (ProbContainer cont in itemList)
        {
            if (picked < cont.num)
            {
                itemList.RemoveAt(i);
                return cont.item;
            }
            i++;
        }
        return default(T);
    }

    List<T> Get(List<int> randNums)
    {
        List<T> ret = new List<T>();
        List<int> removeIndices = new List<int>();
        foreach (ProbContainer cont in itemList)
        {
            foreach (int picked in randNums)
            {
                if (picked < cont.num)
                {
                    ret.Add(cont.item);
                    removeIndices.Add(picked);
                }
            }
            if (removeIndices.Count > 0)
            {
                foreach (int remove in removeIndices)
                {
                    randNums.Remove(remove);
                }
                removeIndices.Clear();
            }
        }
        return ret;
    }

    public List<T> GetUnique(int amount)
    {
        List<T> ret = new List<T>();
        ProbabilityList<T> copy = new ProbabilityList<T>(this);
        for (int i = 0; i < amount; i++)
        {
            T item = copy.GetRemove();
            if (item != null)
            {
                ret.Add(item);
            }
        }
        return ret;
    }

    public List<T> Get(int amount)
    {
        List<int> randNums = new List<int>();
        for (int i = 0; i < amount; i++)
        {
            randNums.Add(rand.Next(maxNum));
        }
        return Get(randNums);
    }

    class ProbContainer {
        public T item;
        private int num_ = 0;
        public int num { get; private set; }

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
