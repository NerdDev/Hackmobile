using UnityEngine;
using System.Collections;
using System;

public class Point {

	public int x {
		get;
		set;
	}
	public int y {
		get;
		set;
	}
	
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
	
	public void shift(int x, int y)
	{
		this.x += x;
		this.y += y;
	}

    public void shift(float x, float y)
    {
        shift((int)x, (int)y);
    }

    public void shift(Vector2 vect)
    {
        shift(vect.x, vect.y);
    }
	
	public void shift(Point p)
	{
		shift (p.x, p.y);	
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
}
