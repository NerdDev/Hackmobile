using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Counter
{
    public int Count;
    public DrawAction<T> Action<T>()
    {
        return new DrawAction<T>((arr2, x, y) =>
        {
            Count++;
            return true;
        });
    }

    public Counter()
    {

    }

    public Counter(int count)
    {
        this.Count = count;
    }

    public static implicit operator int(Counter c)
    {
        return c.Count;
    }

    public override string ToString()
    {
        return Count.ToString();
    }

    public static Counter operator --(Counter c)
    {
        c.Count--;
        return c;
    }

    public static Counter operator ++(Counter c)
    {
        c.Count++;
        return c;
    }
}
