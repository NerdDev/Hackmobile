using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RandomPicker<T>
{
    Container2D<T> options;

    public RandomPicker(T[,] arr)
    {
        if (arr.Length >= 2025) //45 * 45
        {
            this.options = new MultiMap<T>();
        }
        else
        {
            this.options = new ArrayMultiMap<T>(arr.GetLength(0), arr.GetLength(1));
        }
    }

    public bool DrawingAction(T[,] arr, int x, int y)
    {
        options.Put(arr[y, x], x, y);
        return true;
    }

    public List<Value2D<T>> Pick(System.Random rand, int amount, int radius = 0, bool take = false, bool restore = true)
    {
        return options.Random(rand, amount, radius, take);
    }
}
