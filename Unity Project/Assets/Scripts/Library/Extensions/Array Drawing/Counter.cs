using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Counter
{
    private int _count;
    public int Count { get { return _count; } }
    public DrawAction<T> Action<T>(T item)
    {
        return new DrawAction<T>((arr2, x, y) =>
        {
            if (!arr2[y, x].Equals(item))
                _count++;
            return true;
        });
    }
    public static implicit operator int(Counter c)
    {
        return c._count;
    }
}
