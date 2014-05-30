using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ProbabilityTakeReverter<T> : IDisposable
{
    ProbabilityPool<T> pool;

    public ProbabilityTakeReverter(ProbabilityPool<T> pool)
    {
        this.pool = pool;
        this.pool.BeginTaking();
    }

    public void Dispose()
    {
        this.pool.EndTaking();
    }
}