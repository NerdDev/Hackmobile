using UnityEngine;
using System.Collections;
using System;

public class Bounding
{

    private static int max = Int32.MaxValue / 2;
    private static int min = Int32.MinValue / 2;
    public int XMin { get; set; }
    public int XMax { get; set; }
    public int YMin { get; set; }
    public int YMax { get; set; }
	public int Width {
		get { return XMax - XMin; }
	}
	public int Height {
		get { return YMax - YMin; }
	}
	public int Area {
		get { return Width * Height; }
	}

    #region Ctors
    public Bounding()
    {
        XMin = Int32.MaxValue;
        XMax = Int32.MinValue;
        YMin = Int32.MaxValue;
        YMax = Int32.MinValue;
    }

    public Bounding(int xl, int xr, int yb, int yt)
    {
        XMin = xl;
        XMax = xr;
        YMin = yb;
        YMax = yt;
    }

    public Bounding(Point leftdownOrigin, int width, int height)
        : this (leftdownOrigin.x, leftdownOrigin.x + width - 1, 
        leftdownOrigin.y, leftdownOrigin.y + height - 1)
    {
        
    }

    public Bounding(Bounding rhs)
        : this(rhs.XMin, rhs.XMax, rhs.YMin, rhs.YMax)
    {
    }
    #endregion Ctors

    public bool IsValid()
    {
        return XMin > min && XMin < max;
    }
	
	public Point GetCenter()
	{
		if (IsValid())
			return new Point(XMin + Width / 2, YMin + Height / 2);
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
		absorb(rhs.XMin, rhs.YMin);
		absorb(rhs.XMax, rhs.YMax);
	}

    public void absorbX(int x)
    {
        if (XMin > x)
        {
            XMin = x;
        }
        if (XMax < x)
        {
            XMax = x;
        }
    }

    public void absorbY(int y)
    {
        if (YMin > y)
        {
            YMin = y;
        }
        if (YMax < y)
        {
            YMax = y;
        }
    }
    #endregion Absorbs

    #region Intersects
    public void IntersectingDimensions(Bounding rhs, out int width, out int height)
    {
        if (IsValid() && rhs.IsValid())
        {
            IntersectingWidth(rhs, out width);
            IntersectingHeight(rhs, out height);
        }
        else
        {
            width = 0;
            height = 0;
        }
    }

    // Returns the min number, and whether thisNum was the min.
    public bool GetMinDim(int thisNum, int rhsNum, out int result)
    {
        int thisAbs = Math.Abs(thisNum) + 1;
        int rhsAbs = Math.Abs(rhsNum) + 1;
        if (thisAbs < rhsAbs)
        {
            result = thisAbs;
            if (Math.Sign(thisNum) < 0)
            {
                result = -result;
            }
            return true;
        }
        result = rhsAbs;
        if (Math.Sign(rhsNum) < 0)
        {
            result = -result;
        }
        return false;
    }

    // True if completely contained in RHS horizontally
    public bool InsideHoriz(Bounding rhs)
    {
        return XMin > rhs.XMin && XMax < rhs.XMax;
    }

    // True if completely contained in RHS vertically
    public bool InsideVert(Bounding rhs)
    {
        return YMin > rhs.YMin && YMax < rhs.YMax;
    }

    // Gets the minimum intersection outHeight, and leftmost point
    public int IntersectingWidth(Bounding rhs, out int outWidth)
    {
        if (InsideHoriz(rhs))
        { // If completely inside RHS
            outWidth = Width;
            return XMin;
        }
        if (rhs.InsideHoriz(this))
        { // If completely containing RHS
            outWidth = rhs.Width;
            return rhs.XMin;
        }
        return GetMinDim(rhs.XMax - XMin, XMax - rhs.XMin, out outWidth) ? XMin : rhs.XMin;
    }

    // Gets the minimum intersection outHeight, and downmost point
    public int IntersectingHeight(Bounding rhs, out int outHeight)
    {
        if (InsideVert(rhs))
        { // If completely inside RHS
            outHeight = Height;
            return YMin;
        }
        if (rhs.InsideVert(this))
        { // If completely containing RHS
            outHeight = rhs.Height;
            return rhs.YMin;
        }
        return GetMinDim(rhs.YMax - YMin, YMax - rhs.YMin, out outHeight) ? YMin : rhs.YMin;
    }

    public int IntersectArea(Bounding rhs)
    {
        int width;
        int height;
        IntersectingDimensions(rhs, out width, out height);

        // If either x or y intersect is negative, there's no intersection
		if (width > 0 && height > 0)
		{
			return width * height;
		}
		return 0;
    }

    public bool Intersects(Bounding rhs)
    {
        return IntersectArea(rhs) > 0;
    }

    // Returns bounding box of area, but positioning is on the origin
    public Bounding IntersectBoundRelative(Bounding rhs)
    {
        Bounding ret = new Bounding();
        int width;
        int height;
        IntersectingDimensions(rhs, out width, out height);
        ret.XMax = width;
        ret.YMax = height;
        return ret;
    }

    // Returns bounding box of intersecting area
    public Bounding IntersectBounds(Bounding rhs)
    {
        int width, height;
        int leftmost = IntersectingWidth(rhs, out width);
        int downmost = IntersectingHeight(rhs, out height);
        Bounding ret = new Bounding(new Point(leftmost, downmost), width, height);
        return ret;
    }

    // Gets center point of intersecting bounds
    public Point GetCenterPoint(Bounding rhs)
    {
        Bounding intersection = IntersectBounds(rhs);
        return intersection.GetCenter();
    }
    #endregion Intersects

    public void expand(int amount)
    {
        if (IsValid())
        {
            XMax += amount;
            YMax += amount;
            XMin -= amount;
            YMin -= amount;
        }
    }

    public Point GetShiftNonNeg(int buffer)
    {
        Point shift = new Point();
        if (IsValid())
        {
            if (XMin < buffer)
            {
                shift.x = buffer - XMin;
            }
            if (YMin < buffer)
            {
                shift.y = buffer - YMin;
            }
        }
        return shift;
    }

    public Point GetShiftNonNeg()
    {
        return GetShiftNonNeg(0);
    }

    public void Shift(Point shift)
    {
        if (IsValid())
        {
            XMin += shift.x;
            XMax += shift.x;
            YMin += shift.y;
            YMax += shift.y;
        }
    }

    public void ShiftNonNeg()
    {
        Shift(GetShiftNonNeg());
    }

    public Bounding InBounds<T>(T[,] arr)
    {
        return IntersectBounds(arr.GetBounds());
    }

    #region Printing
    public override string ToString()
    {
        return "(" + XMin + "," + YMin + ") (" + XMax + "," + YMax + ")";
    }
    #endregion Printing
}
