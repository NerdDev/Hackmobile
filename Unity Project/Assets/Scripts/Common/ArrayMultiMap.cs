using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ArrayMultiMap<T> : Container2D<T>
{
    int _count = 0;
    public int Count { get { return _count; } }
    bool[,] _present;
    T[,] _arr;
    bool _accurateBounding = false;
    Bounding bound;

    public ArrayMultiMap(int width, int height)
    {
        _arr = new T[height, width];
        _present = new bool[height, width];
    }

    protected override T Get(int x, int y)
    {
        return _arr[y, x];
    }

    public override bool InRange(int x, int y)
    {
        return _arr.InRange(x, y);
    }

    protected override void PutInternal(T val, int x, int y)
    {
        _arr[y, x] = val;
        _present[y, x] = true;
        _accurateBounding = false;
        _count++;
    }

    public void Remove(int x, int y)
    {
        _arr[y, x] = default(T);
        _present[y, x] = false;
        _accurateBounding = false;
        _count++;
    }

    public override Bounding GetBounding()
    {
        if (!_accurateBounding)
        {
            bound = new Bounding();
            for (int y = 0; y < _arr.GetLength(0); y++)
                for (int x = 0; x < _arr.GetLength(1); x++)
                    bound.absorb(x, y);
            _accurateBounding = true;
        }
        return bound;
    }

    public override T[,] GetArr()
    {
        return _arr;
    }

    public T Random(System.Random random)
    {
        int picked = random.Next(_count);

    }
}
