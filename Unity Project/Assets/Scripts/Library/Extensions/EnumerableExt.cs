using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class EnumerableExt
{
    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> e, System.Random rand)
    {
        return e.OrderBy<T, int>((item) => rand.Next());
    }

    public static Type GetEnumeratedType<T>(this IEnumerable<T> _)
    {
        return typeof(T);
    }

    public static IEnumerable<T> Filter<T>(this IEnumerable<T> rhs, Func<T, bool> filter)
    {
        foreach (T t in rhs)
        {
            if (filter(t))
            {
                yield return t;
            }
        }
    }
}
