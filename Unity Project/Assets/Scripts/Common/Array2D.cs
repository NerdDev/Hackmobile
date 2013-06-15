using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Array2Dcoord<T> : Container2D<T>, IEnumerable<Value2D<T>> {

    static int growth = 10;
    protected T[,] arr;
    int xShift;
    int yShift;

    #region Ctors
    protected Array2Dcoord() : base()
    {
    }

    public Array2Dcoord(int width, int height) : base()
    {
        arr = new T[height * 2 + 1, width * 2 + 1];
        xShift = width;
        yShift = height;
    }

    public Array2Dcoord(int width, int height, Array2Dcoord<T> rhs)
        : this(width, height)
    {
        PutAll(rhs);
    }

    public Array2Dcoord(int width, int height, Array2Dcoord<T> rhs, Point shift)
        : this(width, height)
    {
        PutAll(rhs, shift);
    }

    public Array2Dcoord(int width, int height, Array2Dcoord<T> rhs, int xShift, int yShift)
        : this(width, height)
    {
        PutAll(rhs, xShift, yShift);
    }
	
	public Array2Dcoord(Bounding bound)
		: this(Mathf.Max(Mathf.Abs(bound.xMin), Mathf.Abs(bound.xMax))
			,Mathf.Max(Mathf.Abs(bound.yMin), Mathf.Abs(bound.yMax)))
	{
	}
    #endregion

    #region GetSet
    public override T Get(int x, int y)
    {
        x += xShift;
        y += yShift;
        if (InRangeInternal(x, y))
        {
            return arr[y, x];
        }
        return default(T);
    }

    public override bool InRange(int x, int y)
    {
        x += xShift;
        y += yShift;
        return InRangeInternal(x, y);
    }

    protected bool InRangeInternal(int x, int y)
    {
        return y < arr.GetLength(0)
            && x < arr.GetLength(1)
            && y >= 0
            && x >= 0;
    }

    public int getWidth()
    {
        return arr.GetLength(1) - xShift - 1;
    }

    public int getHeight()
    {
        return arr.GetLength(0) - yShift - 1;
    }
	
    public override void Put(T val, int x, int y)
    {
        if (InRange(x, y))
        {
			x += xShift;
			y += yShift;
            if (comparator == null || 1 == comparator.compare(val, arr[y, x]))
            {
                arr[y, x] = val;
            }
        }
        else
        {
            expandToFit(x, y);
            Put(val, x, y);
        }
    }

    public void expandToFit(int x, int y)
    {
		int maxWidth = Math.Max(Math.Abs(x), getWidth ());
		int maxHeight = Math.Max (Math.Abs (y), getHeight());
		if (maxWidth != getWidth()) {
			maxWidth += growth;	
		}
		if (maxHeight != getHeight()) {
			maxHeight += growth;	
		}
        Array2Dcoord<T> tmp = new Array2Dcoord<T>(maxWidth, maxHeight, this);
        arr = tmp.arr;
        xShift = tmp.xShift;
        yShift = tmp.yShift;
    }
	
    public override void PutInternal(T val, int x, int y)
    {
		x += xShift;
		x += yShift;
        arr[y, x] = val;
    }

    public void PutAll(Array2Dcoord<T> rhs)
    {
        for (int y = 0; y < rhs.arr.GetLength(0); y++)
        {
            for (int x = 0; x < rhs.arr.GetLength(1); x++)
            {
                Put(rhs.arr[y, x], x - rhs.xShift, y - rhs.yShift);
            }
        }
    }

    public void PutAll(Array2Dcoord<T> rhs, int additionalXshift, int additionalYshift)
    {
        for (int y = 0; y < rhs.arr.GetLength(0); y++)
        {
            for (int x = 0; x < rhs.arr.GetLength(1); x++)
            {
                Put(rhs.arr[y, x], x - rhs.xShift + additionalXshift, y - rhs.yShift + additionalYshift);
			}
        }
    }

    public void PutAll(Array2Dcoord<T> rhs, Point shift)
    {
        PutAll(rhs, shift.x, shift.y);
    }

    public void PutRow(T t, int xl, int xr, int y)
    {
        xl += xShift;
        xr += xShift;
        y += yShift;
        for (; xl <= xr; xl++)
        {
            arr[y, xl] = t;
        }
    }

    public override void PutCol(T t, int y1, int y2, int x)
    {
        x += xShift;
        y1 += yShift;
        y2 += yShift;
        for (; y1 <= y2; y1++)
        {
            arr[y1, x] = t;
        }
    }

    public override Bounding GetBounding()
    {
        Bounding ret = new Bounding();
        ret.xMin = -xShift;
        ret.xMax = arr.GetLength(1) - xShift;
        ret.yMin = -yShift;
        ret.yMax = arr.GetLength(0) - yShift;
        return ret;
    }

    public override T[,] GetArr()
    {
        return arr;
    }
    #endregion

    #region Iteration
    public IEnumerator<Value2D<T>> GetEnumerator()
    {
        for (int y = 0; y < arr.GetLength(1); y++)
        {
            for (int x = 0; x < arr.GetLength(0); x++)
            {
                Value2D<T> val = new Value2D<T>(x - xShift, y - yShift, arr[y, x]);
                yield return val;
            }
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
    #endregion
}
