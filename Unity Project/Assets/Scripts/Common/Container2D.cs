using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class Container2D<T> {

    protected Comparator<T> comparator;
	
    protected Container2D()
    {
		comparator = getDefaultComparator();
    }
	protected virtual Comparator<T> getDefaultComparator()
	{	
		return null;
	}
    public abstract T Get(int x, int y);
    public abstract bool InRange(int x, int y);
    public virtual void Put(T val, int x, int y)
    {
        if (
			InRange(x, y)
			&& (comparator == null
            || 1 == comparator.compare(val, Get(x,y)))) {
            PutInternal(val, x, y);
        }
    }
    public abstract void PutInternal(T val, int x, int y);
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
	
	public List<String> ToRowStrings()
	{
		T[,] array = GetArr ();
        List<string> ret = new List<string>();
		for (int y = array.GetLength(0) - 1; y >= 0; y -= 1) {
            string rowStr = "";
    		for (int x = 0; x < array.GetLength(1); x += 1) {
        		rowStr += array[y,x].ToString();
    		}
            ret.Add(rowStr);
		}
        return ret;	
	}
	
	public virtual void toLog(DebugManager.Logs log)
    {
        if (DebugManager.logging(log))
        {
			DebugManager.printHeader(log, ToString()); 
            foreach (string s in ToRowStrings())
            {
                DebugManager.w(log, s);
            }
            DebugManager.w(log, "Bounds: " + GetBounding().ToString());
			DebugManager.printFooter(log);
        }
    }

}
