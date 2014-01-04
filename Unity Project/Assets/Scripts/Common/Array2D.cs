using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Array2D<T> : Container2D<T>
{
    bool _validCount = false;
    int _count = 0;
    protected T[,] arr;
    protected bool[,] present;

    #region Ctors
    protected Array2D()
        : base()
    {
    }

    public Array2D(int width, int height)
        : base()
    {
        arr = new T[height, width];
    }

    public Array2D(Container2D<T> rhs)
        : base(rhs)
    {
    }

    public Array2D(Array2D<T> rhs)
        : this(rhs.Width, rhs.Height, rhs)
    {
    }

    public Array2D(int width, int height, Array2D<T> rhs)
        : this(width, height)
    {
        PutAll(rhs);
    }

    public Array2D(int width, int height, Array2D<T> rhs, Point shift)
        : this(width, height)
    {
        PutAll(rhs, shift);
    }

    public Array2D(int width, int height, Array2D<T> rhs, int xShift, int yShift)
        : this(width, height)
    {
        PutAll(rhs, xShift, yShift);
    }

    public Array2D(Bounding bound, bool minimize)
        : this()
    {
        arr = BoundedArr(bound, minimize);
    }
    #endregion

    protected static T[,] BoundedArr(Bounding bound, bool minimize)
    {
        int width = 0;
        int height = 0;
        if (bound.IsValid())
        {
            if (minimize)
            {
                width = bound.Width + 1;
                height = bound.Height + 1;
            }
            else
            {
                width = bound.XMax + 1;
                height = bound.YMax + 1;
            }
        }
        return new T[height, width];
    }

    public override List<string> ToRowStrings()
    {
        return Nifty.AddRuler(arr.ToRowStrings(), Bounding);
    }

    #region GetSet
    public override bool InRange(int x, int y)
    {
        return y < arr.GetLength(0)
            && x < arr.GetLength(1)
            && y >= 0
            && x >= 0;
    }

    public override bool TryGetValue(int x, int y, out T val)
    {
        if (InRange(x, y))
        {
            val = arr[y, x];
            return true;
        }
        val = default(T);
        return false;
    }

    public override Array2D<T> Array
    {
        get { return this; }
    }
    #endregion

    #region Iteration
    public IEnumerable<Value2D<T>> IterateNoEdges()
    {
        for (int y = 1; y < arr.GetLength(0) - 1; y++)
        {
            for (int x = 1; x < arr.GetLength(1) - 1; x++)
            {
                var val = new Value2D<T>(x, y, arr[y, x]);
                yield return val;
            }
        }
    }

    public override IEnumerator<Value2D<T>> GetEnumerator()
    {
        for (int y = 0; y < arr.GetLength(0); y++)
        {
            for (int x = 0; x < arr.GetLength(1); x++)
            {
                var val = new Value2D<T>(x, y, arr[y, x]);
                yield return val;
            }
        }
    }
    #endregion

    public static void invert(Array2D<bool> arr)
    {
        foreach (Value2D<bool> val in arr)
        {
            arr[val] = !val.val;
        }
    }

    public override T this[int x, int y]
    {
        get
        {
            return arr[y, x];
        }
        set
        {
            _validCount = false;
            arr[y, x] = value;
        }
    }

    public override int Count
    {
        get
        {
            if (!_validCount)
            {
                _count = 0;
                T def = default(T);
                for (int y = 0; y < arr.GetLength(0); y++)
                {
                    for (int x = 0; x < arr.GetLength(1); x++)
                    {
                        T val = arr[y, x];
                        if (val != null && !val.Equals(def))
                        {
                            _count++;
                        }
                    }
                }
                _validCount = true;
            }
            return _count;
        }
    }

    public override Bounding Bounding
    {
        get
        {
            Bounding ret = new Bounding();
            ret.XMin = 0;
            ret.XMax = arr.GetLength(1) - 1;
            ret.YMin = 0;
            ret.YMax = arr.GetLength(0) - 1;
            return ret;
        }
    }

    public override bool Contains(int x, int y)
    {
        T val = arr[y, x];
        return val != null && !val.Equals(default(T));
    }

    public override bool Remove(int x, int y)
    {
        bool contained = Contains(x, y);
        arr[y, x] = default(T);
        return contained;
    }

    public Point Minimize(int buffer)
    {
        Bounding bounds = new Bounding(Bounding);
        bounds.expand(buffer);
        bounds.ShiftNonNeg();
        Array2D<T> tmp = new Array2D<T>(this);
        arr = BoundedArr(bounds, true);
        PutAll(tmp, -bounds.XMin, -bounds.YMin);
        return new Point(bounds.XMin, bounds.YMin);
    }

    public override void Clear()
    {
        _validCount = false;
        arr = new T[arr.GetLength(0), arr.GetLength(1)];
        present = new bool[arr.GetLength(0), arr.GetLength(1)];
    }
}
