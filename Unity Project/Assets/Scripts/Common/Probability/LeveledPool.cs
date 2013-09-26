using System;
using System.Collections.Generic;

public class LeveledPool<T> : ProbabilityPool<T>
{
    public LeveledPool(RandomGen rand)
        : base(rand)
    {
    }

    public LeveledPool()
        : base()
    {
    }

    public void Add(T item, float probDiv, bool unique, int level)
    {
        
    }

    public void Add(T item)
    {
        if (item is ProbabilityLevItem)
        {
            ProbabilityLevItem p = (ProbabilityLevItem)item;
            Add(item, p.ProbabilityDiv(), p.IsUnique(), p.GetLevel());
        }
        base.Add(item);
    }

    public override void Add(T item, float probDiv, bool unique)
    {
        Add(item, probDiv, unique, 1);
    }

    public override bool Get(out T item)
    {

    }

    public override bool GetUnique(out T item)
    {
        throw new NotImplementedException();
    }

    public override ProbabilityPool<T> Filter(Func<T, bool> filter)
    {
        throw new NotImplementedException();
    }

    public override void ClearUnique()
    {
        throw new NotImplementedException();
    }

    public override void ToLog(DebugManager.Logs log)
    {
        throw new NotImplementedException();
    }
}
