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
}
