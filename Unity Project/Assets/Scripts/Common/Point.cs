using UnityEngine;
using System.Collections;
using System;

public class Point {

	public int x { get; set; }
	public int y { get; set; }

    #region Ctors
    public Point() {
		x = 0;
		y = 0;
	}

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Point(float x, float y)
    {
        this.x = (int) Math.Round(x);
        this.y = (int) Math.Round(y);
    }
	
	public Point(Vector2 vect) : this(vect.x, vect.y)
	{
		
	}
	
	public Point (Point rhs) : this(rhs.x, rhs.y)
	{
			
	}
    #endregion Ctors

    #region Shifts
    public void Shift(int x, int y)
	{
		this.x += x;
		this.y += y;
	}

    public void Shift(float x, float y)
    {
        Shift((int)x, (int)y);
    }

    public void Shift(Vector2 vect)
    {
        Shift(vect.x, vect.y);
    }
	
	public void Shift(Point p)
	{
		Shift (p.x, p.y);	
	}
    #endregion Shifts

    public Point Invert()
    {
        return new Point(-x, -y);
    }

    public bool isZero()
	{
		return x == 0 && y == 0;	
	}
	
	public override string ToString ()
	{
		return "(" + x + "," + y + ")";
	}
	
	public Point reduce()
	{
		int gcd = Nifty.GCD(x, y);
		return new Point(x / gcd, y / gcd);
	}

    public void Take(out int x, out int y)
    {
        x = Math.Sign(this.x);
        y = Math.Sign(this.y);
        this.x -= x;
        this.y -= y;
    }

    public override bool Equals(object obj)
    {
        Point rhs = obj as Point;
        if (rhs == null) return false;
        if (x != rhs.x || y != rhs.y)
            return false;
        return true;
    }

    public override int GetHashCode()
    {
        int hash = 3;

        hash = 5 * hash + x;
        hash = 11 * hash + y;
        
        return hash;
    }

    public static Point operator +(Point p1, Point p2)
    {
        Point tmp = new Point(p1);
        tmp.Shift(p2);
        return tmp;
    }

    public static Point operator -(Point p1, Point p2)
    {
        Point tmp = new Point(p1);
        tmp.x -= p2.x;
        tmp.y -= p2.y;
        return tmp;
    }
}
