using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class Container2D<T> {

    protected Container2D()
    {
    }
    public T this[int x, int y]
    {
        get
        {
            return Get(x, y);
        }
        set
        {
            Put(value, x, y);
        }
    }
    public T this[Value2D<T> val]
    {
        get
        {
            return Get(val.x, val.y);
        }
        set
        {
            Put(value, val.x, val.y);
        }
    }
    public T this[Point val]
    {
        get
        {
            return Get(val.x, val.y);
        }
        set
        {
            Put(value, val.x, val.y);
        }
    }
    protected abstract T Get(int x, int y);
    public bool ContainsPoint(Value2D<T> val)
    {
        return !Get(val.x, val.y).Equals(default(T));
    }
    public abstract bool InRange(int x, int y);
    public virtual void Put(T val, int x, int y)
    {
        if (InRange(x, y))
            PutInternal(val, x, y);
    }
    public virtual void Put(Value2D<T> val)
    {
        this[val] = val.val;
    }
    protected abstract void PutInternal(T val, int x, int y);
    public abstract Bounding GetBounding();
    public virtual void putRow(T t, int xl, int xr, int y)
    {
        for (; xl <= xr; xl++)
        {
            Put(t, xl, y);
        }
    }
    public virtual void PutCol(T t, int y1, int y2, int x)
    {
        for (; y1 <= y2; y1++)
        {
            Put(t, x, y1);
        }
    }
    public virtual void putSquare(T t, int xl, int xr, int yb, int yt)
    {
        for (; yb <= yt; yb++)
        {
            putRow(t, xl, xr, yb);
        }
    }

    public abstract T[,] GetArr();
	
	public virtual List<string> ToRowStrings()
	{
	    return GetArr().ToRowStrings();
	}
	
	public virtual void ToLog(Logs log)
    {
        if (BigBoss.Debug.logging(log))
        {
            ToLog(log, new string[0]);
        }
    }

    public virtual void ToLog(Logs log, params string[] customContent)
    {
        if (BigBoss.Debug.logging(log))
        {
            BigBoss.Debug.printHeader(log, ToString());
            foreach (string s in customContent)
            {
                BigBoss.Debug.w(log, s);
            }
            foreach (string s in ToRowStrings())
            {
                BigBoss.Debug.w(log, s);
            }
            BigBoss.Debug.w(log, "Bounds: " + GetBounding().ToString());
            BigBoss.Debug.printFooter(log);
        }
    }

    public static void Smallest<Z>(Z obj1, Z obj2, out Z smallest, out Z largest) where Z : Container2D<T>
    {
        if (obj1.GetBounding().Area < obj2.GetBounding().Area)
        {
            smallest = obj1;
            largest = obj2;
            return;
        }
        smallest = obj2;
        largest = obj1;
    }
}
