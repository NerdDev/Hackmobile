using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Counter
{
    private int _count;
    public int Count { get { return _count; } }
    public DrawAction<T> Action<T>()
    {
        return new DrawAction<T>((arr2, x, y) =>
        {
            _count++;
            return true;
        });
    }
    public static implicit operator int(Counter c)
    {
        return c._count;
    }

    public override string ToString()
    {
        return Count.ToString();
    }
}
