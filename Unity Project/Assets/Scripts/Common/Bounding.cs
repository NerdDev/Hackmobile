using UnityEngine;
using System.Collections;
using System;

public class Bounding {

    public int xMin { get; set; }
    public int xMax { get; set; }
    public int yMin { get; set; }
    public int yMax { get; set; }
	public int width {
		get { return xMax - xMin; }
	}
	public int height {
		get { return yMax - yMin; }
	}
	public int area {
		get { return width * height; }
	}

    #region Ctors
    public Bounding()
    {
        xMin = Int32.MaxValue;
        xMax = Int32.MinValue;
        yMin = Int32.MaxValue;
        yMax = Int32.MinValue;
    }

    public Bounding(int xl, int xr, int yb, int yt)
    {
        xMin = xl;
        xMax = xr;
        yMin = yb;
        yMax = yt;
    }

    public Bounding(Bounding rhs)
        : this(rhs.xMin, rhs.xMax, rhs.yMin, rhs.yMax)
    {
    }
    #endregion Ctors

    public bool isValid()
    {
        return xMin != Int32.MaxValue
            && yMin != Int32.MaxValue;
    }
	
	public Point getCenter()
	{
		if (isValid())
			return new Point(xMin + width / 2, yMin + height / 2);
		else
			return new Point();
	}
	
    #region Absorbs
    public void absorb(int x, int y)
    {
        absorbX(x);
        absorbY(y);
    }

    public void absorb<T>(Value2D<T> val)
    {
        absorb(val.x, val.y);
    }
	
	public void absorb(Bounding rhs)	
	{
		absorb(rhs.xMin, rhs.yMin);
		absorb(rhs.xMax, rhs.yMax);
	}

    public void absorbX(int x)
    {
        if (xMin > x)
        {
            xMin = x;
        }
        if (xMax < x)
        {
            xMax = x;
        }
    }

    public void absorbY(int y)
    {
        if (yMin > y)
        {
            yMin = y;
        }
        if (yMax < y)
        {
            yMax = y;
        }
    }
    #endregion Absorbs

    #region Intersects
    public void boundingDimensions(Bounding rhs, out int width, out int height)
    {
        width = System.Math.Min(xMax - rhs.xMin, rhs.xMax - xMin) + 1;
        height = System.Math.Min(yMax - rhs.yMin, rhs.yMax - yMin) + 1;
    }

    public int intersectArea(Bounding rhs)
    {
        int width;
        int height;
        boundingDimensions(rhs, out width, out height);

        // If either x or y intersect is negative, there's no intersection
		if (width > 0 && height > 0)
		{
			return width * height;
		}
		return 0;
    }

    public bool intersects(Bounding rhs)
    {
        return intersectArea(rhs) > 0;
    }

    // Returns bounding box of area, but positioning is on the origin
    public Bounding intersectBoundRelative(Bounding rhs)
    {
        Bounding ret = new Bounding();
        int width;
        int height;
        boundingDimensions(rhs, out width, out height);
        ret.xMax = width;
        ret.yMax = height;
        return ret;
    }
    #endregion Intersects

    public void expand(int amount)
    {
        xMax += amount;
        yMax += amount;
        xMin -= amount;
        yMin -= amount;
    }

    #region Printing
    public override string ToString()
    {
        return "(" + xMin + "," + yMin + ") (" + xMax + "," + yMax + ")";
    }
    #endregion Printing
}
