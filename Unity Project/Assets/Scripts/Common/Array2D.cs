using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Array2D<T> : Container2D<T>
{
    protected int count;
    protected T[,] arr;
    protected bool[,] present;
    Point shift;

    #region Ctors
    protected Array2D()
        : base()
    {
        shift = new Point();
    }

    public Array2D(int width, int height)
        : this()
    {
        arr = new T[height, width];
        present = new bool[height, width];
    }

    public Array2D(Bounding bounds)
        : this(bounds.Width, bounds.Height)
    {
        shift.x = bounds.XMin;
        shift.y = bounds.YMin;
    }

    public Array2D(Container2D<T> rhs, Point shift = null)
        : this(rhs.Width, rhs.Height)
    {
        if (rhs is Array2D<T> && shift == null)
        {
            Array2D<T> rhsArr = (Array2D<T>)rhs;
            for (int y = 0; y < rhsArr.Height; y++)
            {
                for (int x = 0; x < rhsArr.Width; x++)
                {
                    arr[y, x] = rhsArr.arr[y, x];
                    present[y, x] = rhsArr.present[y, x];
                }
            }
            shift = new Point(rhsArr.shift);
        }
        else
        {
            if (shift == null)
                PutAll(rhs);
            else
                PutAll(rhs, shift);
        }
    }
    #endregion

    public override List<string> ToRowStrings()
    {
        return Nifty.AddRuler(arr.ToRowStrings(), Bounding);
    }

    #region GetSet
    public override bool InRange(int x, int y)
    {
        x -= shift.x;
        y -= shift.y;
        return y < arr.GetLength(0)
            && x < arr.GetLength(1)
            && y >= 0
            && x >= 0;
    }

    public override bool TryGetValue(int x, int y, out T val)
    {
        if (InRange(x, y))
        {
            val = arr[y - shift.y, x - shift.x];
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
    public override IEnumerator<Value2D<T>> GetEnumerator()
    {
        for (int y = 0; y < arr.GetLength(0); y++)
        {
            for (int x = 0; x < arr.GetLength(1); x++)
            {
                if (present[y, x])
                {
                    var val = new Value2D<T>(x + shift.x, y + shift.y, arr[y, x]);
                    yield return val;
                }
            }
        }
    }
    #endregion

    public override T this[int x, int y]
    {
        get
        {
            T ret;
            TryGetValue(x, y, out ret);
            return ret;
        }
        set
        {
            if (!InRange(x, y))
                ExpandToInclude(x, y);
            x -= shift.x;
            y -= shift.y;
            arr[y, x] = value;
            if (present[y, x])
                count++;
            present[y, x] = true;
        }
    }

    public override int Count { get { return count; } }

    public override Bounding Bounding
    {
        get
        {
            Bounding ret = new Bounding();
            ret.XMin = 0 + shift.x;
            ret.XMax = arr.GetLength(1) - 1 + shift.x;
            ret.YMin = 0 + shift.y;
            ret.YMax = arr.GetLength(0) - 1 + shift.y;
            return ret;
        }
    }

    public override bool Contains(int x, int y)
    {
        return present[y - shift.y, x - shift.x];
    }

    public override bool Remove(int x, int y)
    {
        bool contained = Contains(x, y);
        x -= shift.x;
        y -= shift.y;
        arr[y, x] = default(T);
        if (present[y, x])
            count--;
        present[y, x] = false;
        return contained;
    }

    public override void Clear()
    {
        count = 0;
        arr = new T[arr.GetLength(0), arr.GetLength(1)];
        present = new bool[arr.GetLength(0), arr.GetLength(1)];
    }

    protected void Expand(int left, int right, int top, int bottom)
    {
        if (left == 0 && right == 0 & top == 0 && bottom == 0) return;
        shift.x -= left;
        shift.y -= top;
        T[,] tmp = arr;
        bool[,] tmpPresent = present;
        arr = new T[tmp.GetLength(0) + top + bottom, tmp.GetLength(1) + left + right];
        present = new bool[arr.GetLength(0), arr.GetLength(1)];
        for (int y = 0; y < tmp.GetLength(0); y++)
        {
            for (int x = 0; x < tmp.GetLength(1); x++)
            {
                arr[y, x] = tmp[y + bottom, x + left];
                present[y, x] = tmpPresent[y + bottom, x + left];
            }
        }
    }

    protected void ExpandToInclude(int x, int y)
    { // external space
        int left = 0, right = 0, top = 0, bottom = 0;
        x -= shift.x;
        y -= shift.y;
        if (x < 0)
            left = 0 - x;
        else if (x >= Width)
            right = Width - x;
        if (y < 0)
            bottom = 0 - y;
        else if (y >= Height)
            top = Height - y;
        Expand(left, right, top, bottom);
    }
}
