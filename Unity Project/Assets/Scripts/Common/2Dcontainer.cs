using UnityEngine;
using System.Collections;

abstract public class Container2D<T> {

    protected Comparator<T> comparator;

    public abstract T get(int x, int y);
    public abstract bool inRange(int x, int y);
    public virtual void put(T val, int x, int y)
    {
        if (comparator == null
            || 1 == comparator.compare(val, get(x,y))) {
            putInternal(val, x, y);
        }
    }
    public abstract void putInternal(T val, int x, int y);
    public abstract Bounding getBounding();
    public virtual void putRow(T t, int xl, int xr, int y)
    {
        for (; xl <= xr; xl++)
        {
            put(t, xl, y);
        }
    }
    public virtual void putCol(T t, int y1, int y2, int x)
    {
        for (; y1 <= y2; y1++)
        {
            put(t, x, y1);
        }
    }
    public virtual void putSquare(T t, int xl, int xr, int yb, int yt)
    {
        for (; yb <= yt; yb++)
        {
            putRow(t, xl, xr, yb);
        }
    }
    public abstract T[,] getArr();

}
