using UnityEngine;
using System.Collections;
using System;

public class Bounding {

    public int xMin { get; set; }
    public int xMax { get; set; }
    public int yMin { get; set; }
    public int yMax { get; set; }

    #region Ctors
    public Bounding()
    {
        xMin = 0;
        xMax = 0;
        yMin = 0;
        yMax = 0;
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

    #region Absorbs
    public void absorb(int x, int y)
    {
        absorbX(x);
        absorbY(y);
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
        else if (xMax < x)
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
        else if (yMax < y)
        {
            yMax = y;
        }
    }
    #endregion Absorbs

    #region GetSet
    public int width()
	{
		return xMax - xMin;
	}
	
	public int height()
	{
		return yMax - yMin;	
	}

    public int area()
    {
        return width() * height();
    }
    #endregion GetSet

    #region Intersects
    public void boundingDimensions(Bounding rhs, out int width, out int height)
    {
        width = System.Math.Min(xMax - rhs.xMin, rhs.xMax - xMin);
        height = System.Math.Min(yMax - rhs.yMin, rhs.yMax - yMin);
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

    #region UNUSED
    // Returns bounding box of intersection, maintaining correct position.
    public Bounding intersectBoundingAbsolute(Bounding rhs)
    {
        //Bounding leftMost = xMin < rhs.xMin ? this : rhs;
        //Bounding rightMost = xMin > rhs.xMin ? this : rhs;
        //Bounding bottomMost = yMin < rhs.yMin ? this : rhs;
        //Bounding topMost = yMin > rhs.yMin ? this : rhs;
        
        //Bounding ret = new Bounding();

        throw new NotImplementedException();
    }
    #endregion

    #region Printing
    public override string ToString()
    {
        return "(" + xMin + "," + yMin + ") (" + xMax + "," + yMax + ")";
    }
    #endregion Printing
}
