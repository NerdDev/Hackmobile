using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RandomPicker<T>
{
    ArrayMultiMap<T> options;

    public RandomPicker(T[,] arr)
    {
        this.options = new ArrayMultiMap<T>(arr.GetLength(0), arr.GetLength(1));
    }

    public bool DrawingAction(T[,] arr, int x, int y)
    {
        options.Put(arr[y, x], x, y);
        return true;
    }

    public List<Value2D<T>> Pick(System.Random rand, int amount, int radius = 0, bool take = false)
    {
        return options.Random(rand, amount, radius, take);
    }
}
