using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// Array2Dcoord
// An x-y coordinate container with multidimensional array internal storage.
// (0,0) is stored at total width / 2 and total height / 2, respectively.
// Multidimensional array is indexed [y,x].
// Array automatically expands if a value is put outside its range.
public class Array2Dcoord<T> : Container2D<T>, IEnumerable<Value2D<T>> {

    static int growth = 10;
    protected T[,] arr;
    int xShift;
    int yShift;

    #region Ctors
    protected Array2Dcoord() : base()
    {
    }

    // Creates an array with width/height in both directions
    // away from the origin (0,0)
    public Array2Dcoord(int width, int height) : base()
    {
        arr = new T[height * 2 + 1, width * 2 + 1];
        xShift = width;
        yShift = height;
    }

    // Copy Ctor
    public Array2Dcoord(int width, int height, Array2Dcoord<T> rhs)
        : this(width, height)
    {
        PutAll(rhs);
    }

    // Copy Ctor.  Also shifts points given amount
    public Array2Dcoord(int width, int height, Array2Dcoord<T> rhs, Point shift)
        : this(width, height)
    {
        PutAll(rhs, shift);
    }

    // Copy Ctor.  Also shifts points given amount
    public Array2Dcoord(int width, int height, Array2Dcoord<T> rhs, int xShift, int yShift)
        : this(width, height)
    {
        PutAll(rhs, xShift, yShift);
    }

    // Creates an array with width/height in both directions from the origin.
    // Uses the max width/height of the bounds in both directions, respectively.
	public Array2Dcoord(Bounding bound)
		: this(Mathf.Max(Mathf.Abs(bound.xMin), Mathf.Abs(bound.xMax))
			,Mathf.Max(Mathf.Abs(bound.yMin), Mathf.Abs(bound.yMax)))
	{
	}
    #endregion

    #region GetSet
    public override T Get(int x, int y)
    {
        // Shift public points to proper internal points
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
        // Shift public points to proper internal points
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

    // Returns how far the array stores away from
    // the origin in the X direction
    public int getWidth()
    {
        return arr.GetLength(1) - xShift - 1;
    }

    // Returns how far the array stores away from
    // the origin in the Y direction
    public int getHeight()
    {
        return arr.GetLength(0) - yShift - 1;
    }
	
    // Puts value into the array
    // If outside the range, a new larger array is
    // made to store the value.
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

    // Expands the internal array to fit the values
    // plus a static growth buffer amount.
    public void expandToFit(int x, int y)
    {
        // Determine largest of parameter values vs existing values
		int maxWidth = Math.Max(Math.Abs(x), getWidth ());
		int maxHeight = Math.Max (Math.Abs (y), getHeight());

        // If dimension needs to be grown, add growth buffer
		if (maxWidth != getWidth()) {
			maxWidth += growth;	
		}
		if (maxHeight != getHeight()) {
			maxHeight += growth;	
		}

        // Make new larger arrray and copy data
        Array2Dcoord<T> tmp = new Array2Dcoord<T>(maxWidth, maxHeight, this);

        // Set internal array to the new copied array
        arr = tmp.arr;
        xShift = tmp.xShift;
        yShift = tmp.yShift;
    }
	
    // Unsafe put that does no checking or expanding
    protected override void PutInternal(T val, int x, int y)
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
                // Have to shift x/y to "public" realm to call public put function
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
                // Have to shift x/y to "public" realm to call public put function
                // Also apply additional desired shifts
                Put(rhs.arr[y, x], x - rhs.xShift + additionalXshift, y - rhs.yShift + additionalYshift);
			}
        }
    }

    // Puts in values from another collection with desired shift
    public void PutAll(Array2Dcoord<T> rhs, Point shift)
    {
        PutAll(rhs, shift.x, shift.y);
    }

    // Fills in a row with a desired value
    public void PutRow(T t, int xl, int xr, int y)
    {
        // Shift public points to proper internal points
        xl += xShift;
        xr += xShift;
        y += yShift;
        for (; xl <= xr; xl++)
        {
            arr[y, xl] = t;
        }
    }

    // Fills in a col with a desired value
    public override void PutCol(T t, int y1, int y2, int x)
    {
        // Shift public points to proper internal points
        x += xShift;
        y1 += yShift;
        y2 += yShift;
        for (; y1 <= y2; y1++)
        {
            arr[y1, x] = t;
        }
    }

    // Gets public bounds of array 
    // (Expanding equally both directions from the origin)
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
