using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Array2D<T> : Container2D<T>
{
    private Bounding _bounding;
    private const int _expandAmount = 10;
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

    public Array2D(Container2D<T> rhs)
        : this(rhs.Bounding)
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
        }
        else
        {
            PutAll(rhs);
        }
    }
    #endregion

    public override List<string> ToRowStrings(Bounding bounds = null)
    {
        if (bounds != null)
            bounds = Bounding;
        return Nifty.AddRuler(arr.ToRowStrings(Bounding.Shift(-shift)), bounds);
    }

    #region GetSet
    public override bool InRange(int x, int y)
    {
        return InRangeInternal(x - shift.x, y - shift.y);
    }

    protected bool InRangeInternal(int x, int y)
    {
        return y < arr.GetLength(0)
            && x < arr.GetLength(1)
            && y >= 0
            && x >= 0;
    }

    public override bool TryGetValue(int x, int y, out T val)
    {
        x -= shift.x;
        y -= shift.y;
        if (InRangeInternal(x, y))
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

    public override bool DrawAll(DrawAction<T> call)
    {
        for (int y = 0; y < arr.GetLength(0); y++)
        {
            for (int x = 0; x < arr.GetLength(1); x++)
            {
                if (present[y, x])
                {
                    if (!call(this, x, y)) return false;
                }
            }
        }
        return true;
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
            x -= shift.x;
            y -= shift.y;
            if (!InRangeInternal(x, y))
            {
                Point expandShift = ExpandToInclude(x, y);
                x -= expandShift.x;
                y -= expandShift.y;
            }
            arr[y, x] = value;
            if (present[y, x])
                count++;
            present[y, x] = true;
            _bounding = null;
        }
    }

    public override int Count { get { return count; } }

    public override Bounding Bounding
    {
        get
        {
            if (_bounding == null)
            {
                _bounding = new Bounding();
                foreach (Value2D<T> val in this)
                {
                    _bounding.Absorb(val);
                }
            }
            return new Bounding(_bounding);
        }
    }

    public override bool Contains(int x, int y)
    {
        x -= shift.x;
        y -= shift.y;
        if (!InRangeInternal(x, y)) return false;
        return present[y, x];
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

    protected Point Expand(int left, int right, int top, int bottom)
    {
        if (left == 0 && right == 0 & top == 0 && bottom == 0) return new Point(0,0);
        T[,] tmp = arr;
        bool[,] tmpPresent = present;
        arr = new T[tmp.GetLength(0) + top + bottom, tmp.GetLength(1) + left + right];
        present = new bool[arr.GetLength(0), arr.GetLength(1)];
        for (int y = 0; y < tmp.GetLength(0); y++)
        {
            for (int x = 0; x < tmp.GetLength(1); x++)
            {
                arr[y + bottom, x + left] = tmp[y, x];
                present[y + bottom, x + left] = tmpPresent[y, x];
            }
        }
        shift.x -= left;
        shift.y -= bottom;
        return new Point(-left, -bottom);
    }

    protected Point ExpandToInclude(int x, int y)
    {
        int left = 0, right = 0, top = 0, bottom = 0;
        if (x < 0)
            left = 0 - x + _expandAmount;
        else if (x >= Width)
            right = x - Width + 1 + _expandAmount;
        if (y < 0)
            bottom = 0 - y + _expandAmount;
        else if (y >= Height)
            top = y - Height + 1 + _expandAmount;
        return Expand(left, right, top, bottom);
    }

    public override Array2DRaw<T> RawArray(out Point shift)
    {
        shift = new Point(this.shift);
        return new Array2DRaw<T>(arr);
    }

    public override void Shift(int x, int y)
    {
        shift.Shift(x, y);
    }
}
