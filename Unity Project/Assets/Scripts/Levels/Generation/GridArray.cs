using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridArray : Array2D<GridType>
{
    #region Ctors
    public GridArray(int width, int height)
        : base(width, height)
    {
    }

    public GridArray(GridArray rhs)
        : base(rhs.arr.GetLength(1), rhs.arr.GetLength(0))
    {
        PutAll(rhs);
    }

    public GridArray(GridArray rhs, Point shift)
        : base(rhs.arr.GetLength(1), rhs.arr.GetLength(0))
    {
        PutAll(rhs, shift);
    }

    public GridArray(GridArray rhs, int xShift, int yShift)
        : base(rhs.arr.GetLength(1), rhs.arr.GetLength(0))
    {
        PutAll(rhs, xShift, yShift);
    }

    public GridArray(Bounding bounds, bool minimize)
        : base(bounds, minimize)
    {
    }
    #endregion

    public override void Put(GridType val, int x, int y)
    {
        if (val != GridType.NULL)
        {
            base.Put(val, x, y);
        }
    }

    public void PutNull(int x, int y)
    {
        base.Put(GridType.NULL, x, y);
    }

    public void PutAll(LayoutObject obj, Bounding origBound)
    {
        Point shift = obj.GetShift();
        shift.x -= origBound.XMin;
        shift.y -= origBound.YMin;
        base.PutAll(obj.GetArray(), shift);
    }
    public void PutAll(LayoutObject rhs)
    {
        PutAll(rhs.GetArray(), rhs.GetShift());
    }

    public void PutAs(GridArray rhs, GridType type)
    {
        foreach (Value2D<GridType> val in rhs)
        {
            Put(type, val.x, val.y);
        }
    }

    public override Bounding GetBounding()
    {
        Bounding ret = new Bounding();
        for (int y = 0; y < getHeight(); y++)
        {
            for (int x = 0; x < getWidth(); x++)
            {
                if (this[x, y] != GridType.NULL)
                {
                    ret.Absorb(x, y);
                }
            }
        }
        return ret;
    }

    public Bounding GetBoundingInternal()
    {
        return base.GetBounding();
    }

    public Point Minimize(int buffer)
    {
        Bounding bounds = GetBounding();
        bounds.expand(buffer);
        bounds.ShiftNonNeg();
        GridType[,] tmp = arr;
        bool[,] tmpPresent = present;
        arr = BoundedArr(bounds, true);
        present = new bool[arr.GetLength(0), arr.GetLength(1)];
        for (int y = 0; y < tmp.GetLength(0); y++)
        {
            for (int x = 0; x < tmp.GetLength(1); x++)
            {
                if (tmpPresent[y, x])
                    Put(tmp[y, x], x - bounds.XMin, y - bounds.YMin);
            }
        }
        return new Point(bounds.XMin, bounds.YMin);
    }

    public static implicit operator GridType[,](GridArray src)
    {
        return src.arr;
    }
}
