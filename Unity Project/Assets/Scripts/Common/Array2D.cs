using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Array2D<T> : Container2D<T>, IEnumerable<Value2D<T>> {

    static int growth = 10;
    protected T[,] arr;

    #region Ctors
    protected Array2D() : base()
    {
    }

    public Array2D(int width, int height) : base()
    {
        arr = new T[height, width];
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

	public Array2D(Bounding bound) : this()
    {
        int width = 0;
        int height = 0;
        if (bound.isValid())
        {
            width = Mathf.Max(Mathf.Abs(bound.xMin), Mathf.Abs(bound.xMax));
            height = Mathf.Max(Mathf.Abs(bound.yMin), Mathf.Abs(bound.yMax));
        }
        arr = new T[height, width];
	}
    #endregion

    #region GetSet
    public override T Get(int x, int y)
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
        // Determine largest of parameter values vs existing values
		int maxWidth = Math.Max(x, getWidth ());
		int maxHeight = Math.Max (y, getHeight());

        // If dimension needs to be grown, add growth buffer
		if (maxWidth != getWidth()) {
			maxWidth += growth;	
		}
		if (maxHeight != getHeight()) {
			maxHeight += growth;	
		}

        // Make new larger arrray and copy data
        Array2D<T> tmp = new Array2D<T>(maxWidth, maxHeight, this);

        // Set internal array to the new copied array
        arr = tmp.arr;
    }
	
    // Unsafe put that does no checking or expanding
    protected override void PutInternal(T val, int x, int y)
    {
        arr[y, x] = val;
    }

    public void PutAll(Array2D<T> rhs)
    {
        for (int y = 0; y < rhs.arr.GetLength(0); y++)
        {
            for (int x = 0; x < rhs.arr.GetLength(1); x++)
            {
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
                Put(rhs.arr[y, x], x + additionalXshift, y + additionalYshift);
			}
        }
    }

    // Puts in values from another collection with desired shift
    public void PutAll(Array2D<T> rhs, Point shift)
    {
        PutAll(rhs, shift.x, shift.y);
    }

    // Fills in a row with a desired value
    public void PutRow(T t, int xl, int xr, int y)
    {
        for (; xl <= xr; xl++)
        {
            arr[y, xl] = t;
        }
    }

    // Fills in a col with a desired value
    public override void PutCol(T t, int y1, int y2, int x)
    {
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
        ret.xMin = 0;
        ret.xMax = arr.GetLength(1) - 1;
        ret.yMin = 0;
        ret.yMax = arr.GetLength(0) - 1;
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
        for (int y = 0; y < arr.GetLength(0); y++)
        {
            for (int x = 0; x < arr.GetLength(1); x++)
            {
                Value2D<T> val = new Value2D<T>(x, y, arr[y, x]); 
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
