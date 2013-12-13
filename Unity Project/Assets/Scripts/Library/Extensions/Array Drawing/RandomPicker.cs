using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RandomPicker<T>
{
    private T[,] _arr;
    private readonly Container2D<T> _options;

    public RandomPicker(T[,] arr)
    {
        _arr = arr;
        if (arr.Length >= 2025) //45 * 45
        {
            _options = new MultiMap<T>();
        }
        else
        {
            _options = new ArrayMultiMap<T>(arr.GetLength(0), arr.GetLength(1));
        }
    }

    public bool DrawingAction(T[,] arr, int x, int y)
    {
        _options.Put(arr[y, x], x, y);
        return true;
    }

    public List<Value2D<T>> Pick(System.Random rand, int amount, int radius = 0, bool take = false)
    {
        return _options.Random(rand, amount, radius, take);
    }

    public Value2D<T> Pick(System.Random rand, bool take = false)
    {
        List<Value2D<T>> list = Pick(rand, 1, 0, take);
        if (list.Count == 0) return null;
        return list[0];
    }

    public void ToLog(Logs logs, params string[] customContent)
    {
        BigBoss.Debug.printHeader(logs, "Random Picker");
        var tmp = new T[_arr.GetLength(0), _arr.GetLength(1)];
        foreach (Value2D<T> val in _options)
            tmp[val.y, val.x] = val.val;
        tmp.ToLog(logs, customContent);
        BigBoss.Debug.printFooter(logs);
    }
}
