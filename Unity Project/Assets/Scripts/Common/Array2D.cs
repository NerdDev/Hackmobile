using UnityEngine;
using System.Collections;

public class Array2Dcoord<T> : Container2D<T> {
    
    protected T[,] arr;
    int xShift;
    int yShift;

    #region Ctors
    protected Array2Dcoord() : base()
    {
    }

    public Array2Dcoord(int width, int height) : base()
    {
        arr = new T[height, width];
        xShift = width / 2;
        yShift = height / 2;
    }

    public Array2Dcoord(int width, int height, Array2Dcoord<T> rhs)
        : this(width, height)
    {
        putAll(rhs);
    }

    public Array2Dcoord(int width, int height, Array2Dcoord<T> rhs, Point shift)
        : this(width, height)
    {
        putAll(rhs, shift);
    }

    public Array2Dcoord(int width, int height, Array2Dcoord<T> rhs, int xShift, int yShift)
        : this(width, height)
    {
        putAll(rhs, xShift, yShift);
    }
	
	public Array2Dcoord(Bounding bound)
		: this((Mathf.Max(Mathf.Abs(bound.xMin), Mathf.Abs(bound.xMax) + 1) * 2)
			,(Mathf.Max(Mathf.Abs(bound.yMin), Mathf.Abs(bound.yMax)) + 1) * 2)
	{
	}
    #endregion

    #region GetSet
    public override T get(int x, int y)
    {
        x += xShift;
        y += yShift;
        if (inRangeInternal(x, y))
        {
            return arr[y, x];
        }
        return default(T);
    }

    public override bool inRange(int x, int y)
    {
        x += xShift;
        y += yShift;
        return inRangeInternal(x, y);
    }

    protected bool inRangeInternal(int x, int y)
    {
        return y < arr.GetLength(0)
            && x < arr.GetLength(1)
            && y >= 0
            && x >= 0;
    }
	
    public override void put(T val, int x, int y)
    {
		x += xShift;
		y += yShift;
        if (inRangeInternal(x, y)
			&& (comparator == null
            || 1 == comparator.compare(val, arr[y,x]))) {
            arr[y,x] = val;
        }
    }
	
    public override void putInternal(T val, int x, int y)
    {
		x += xShift;
		x += yShift;
        arr[y, x] = val;
    }

    public void putAll(Array2Dcoord<T> rhs)
    {
        for (int y = 0; y < rhs.arr.GetLength(0); y++)
        {
            for (int x = 0; x < rhs.arr.GetLength(1); x++)
            {
                put(rhs.arr[y, x], x - rhs.xShift, y - rhs.yShift);
            }
        }
    }

    public void putAll(Array2Dcoord<T> rhs, int additionalXshift, int additionalYshift)
    {
        for (int y = 0; y < rhs.arr.GetLength(0); y++)
        {
            for (int x = 0; x < rhs.arr.GetLength(1); x++)
            {
                put(rhs.arr[y, x], x - rhs.xShift + additionalXshift, y - rhs.yShift + additionalYshift);
			}
        }
    }

    public void putAll(Array2Dcoord<T> rhs, Point shift)
    {
        putAll(rhs, shift.x, shift.y);
    }

    public void putRow(T t, int xl, int xr, int y)
    {
        xl += xShift;
        xr += xShift;
        y += yShift;
        for (; xl <= xr; xl++)
        {
            arr[y, xl] = t;
        }
    }

    public override void putCol(T t, int y1, int y2, int x)
    {
        x += xShift;
        y1 += yShift;
        y2 += yShift;
        for (; y1 <= y2; y1++)
        {
            arr[y1, x] = t;
        }
    }

    public override Bounding getBounding()
    {
        Bounding ret = new Bounding();
        ret.xMin = -xShift;
        ret.xMax = arr.GetLength(1) - xShift;
        ret.yMin = -yShift;
        ret.yMax = arr.GetLength(0) - yShift;
        return ret;
    }

    public override T[,] getArr()
    {
        return arr;
    }
    #endregion
}
