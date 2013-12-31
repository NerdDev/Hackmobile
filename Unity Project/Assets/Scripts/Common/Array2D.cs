using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Array2D<T> : Container2D<T>
{
    protected bool[,] present;
    protected T[,] arr;

    #region Ctors
    protected Array2D()
        : base()
    {
    }

    public Array2D(int width, int height)
        : base()
    {
        arr = new T[height, width];
        present = new bool[height, width];
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
        present = new bool[arr.GetLength(0), arr.GetLength(1)];
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

    #region GetSet
    protected override T Get(int x, int y)
    {
        if (InRange(x, y))
        {
            return arr[y, x];
        }
        return default(T);
    }

    public override bool InRange(int x, int y)
    {
        return y < arr.GetLength(0)
            && x < arr.GetLength(1)
            && y >= 0
            && x >= 0;
    }

    public bool TryGet(int x, int y, out T val)
    {
        if (InRange(x, y))
        {
            val = arr[y, x];
            return true;
        }
        val = default(T);
        return false;
    }

    public int getWidth()
    {
        return arr.GetLength(1);
    }

    public int getHeight()
    {
        return arr.GetLength(0);
    }

    public override void Put(T val, int x, int y)
    {
        if (InRange(x, y))
        {
            arr[y, x] = val;
            present[y, x] = true;
        }
    }

    // Unsafe put that does no checking or expanding
    protected override void PutInternal(T val, int x, int y)
    {
        arr[y, x] = val;
        present[y, x] = true;
    }

    public void PutAll(Array2D<T> rhs)
    {
        for (int y = 0; y < rhs.arr.GetLength(0); y++)
        {
            for (int x = 0; x < rhs.arr.GetLength(1); x++)
            {
                if (rhs.present[y, x])
                    Put(rhs.arr[y, x], x, y);
            }
        }
    }

    public void PutAll(Array2D<T> rhs, int additionalXshift, int additionalYshift)
    {
        for (int y = 0; y < rhs.arr.GetLength(0); y++)
        {
            for (int x = 0; x < rhs.arr.GetLength(1); x++)
            {
                if (rhs.present[y, x])
                    Put(rhs.arr[y, x], x + additionalXshift, y + additionalYshift);
            }
        }
    }

    // Puts in values from another collection with desired shift
    public void PutAll(Array2D<T> rhs, Point shift)
    {
        PutAll(rhs, shift.x, shift.y);
    }

    // Gets public bounds of array 
    // (Expanding equally both directions from the origin)
    public override Bounding GetBounding()
    {
        Bounding ret = new Bounding();
        ret.XMin = 0;
        ret.XMax = arr.GetLength(1) - 1;
        ret.YMin = 0;
        ret.YMax = arr.GetLength(0) - 1;
        return ret;
    }

    public override T[,] GetArr()
    {
        return arr;
    }
    #endregion

    #region Iteration
    public IEnumerable<Value2D<T>> IterateNoEdges()
    {
        for (int y = 1; y < arr.GetLength(0) - 1; y++)
        {
            for (int x = 1; x < arr.GetLength(1) - 1; x++)
            {
                if (present[y, x])
                {
                    var val = new Value2D<T>(x, y, arr[y, x]);
                    yield return val;
                }
            }
        }
    }

    public override IEnumerator<Value2D<T>> GetEnumerator()
    {
        for (int y = 0; y < arr.GetLength(0); y++)
        {
            for (int x = 0; x < arr.GetLength(1); x++)
            {
                if (present[y, x])
                {
                    var val = new Value2D<T>(x, y, arr[y, x]);
                    yield return val;
                }
            }
        }
    }
    #endregion

    public Point Center()
    {
        return new Point(arr.GetLength(1) / 2, arr.GetLength(0) / 2);
    }

    public static void Invert(Array2D<bool> arr)
    {
        foreach (Value2D<bool> val in arr)
        {
            arr.arr[val.y, val.x] = !val.val;
            arr.present[val.y, val.x] = !arr.present[val.y, val.x];
        }
    }
}
