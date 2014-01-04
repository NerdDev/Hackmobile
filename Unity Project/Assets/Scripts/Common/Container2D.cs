using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class Container2D<T> : IEnumerable<Value2D<T>>
{
    protected Container2D()
    {
    }

    public Container2D(Container2D<T> rhs)
    {
        PutAll(rhs);
    }

    public Container2D(Container2D<T> rhs, Point shift)
    {
        PutAll(rhs, shift);
    }

    public Container2D(Container2D<T> rhs, int xShift, int yShift)
    {
        PutAll(rhs, xShift, yShift);
    }

    public abstract T this[int x, int y] { get; set; }
    public T this[Point val]
    {
        get
        {
            return this[val.x, val.y];
        }
        set
        {
            this[val.x, val.y] = value;
        }
    }
    public abstract bool TryGetValue(int x, int y, out T val);
    public bool IsEmpty { get { return Count == 0; } }
    public abstract int Count { get; }
    public abstract Bounding Bounding { get; }
    public int Width { get { return Bounding.Width; } }
    public int Height { get { return Bounding.Height; } }
    public Point Center { get { return Bounding.GetCenter(); } }
    public abstract Array2D<T> Array { get; }
    public abstract bool Contains(int x, int y);
    public bool Contains(Point val)
    {
        return Contains(val.x, val.y);
    }
    public abstract bool InRange(int x, int y);

    public Value2D<T> GetNth(int n)
    {
        if (n >= 0)
        {
            int count = 0;
            foreach (Value2D<T> val in this)
            {
                if (count == n)
                {
                    return val;
                }
                count++;
            }
        }
        return null;
    }

    public abstract void Clear();

    public void Put(Value2D<T> val)
    {
        this[val] = val.val;
    }

    #region Removes
    public bool Remove(Point p)
    {
        return Remove(p.x, p.y);
    }
    public abstract bool Remove(int x, int y);
    public void Remove(int x, int y, int radius)
    {
        for (int yCur = y - radius; yCur <= y + radius; yCur++)
        {
            for (int xCur = x - radius; xCur <= x + radius; xCur++)
            {
                Remove(xCur, yCur);
            }
        }
    }

    public void RemoveAll(Container2D<T> rhs)
    {
        foreach (Value2D<T> val in rhs)
        {
            Remove(val);
        }
    }

    public void RemoveAllBut(params T[] types)
    {
        RemoveAllBut(new HashSet<T>(types));
    }

    public void RemoveAllBut(HashSet<T> types)
    {
        List<Value2D<T>> vals = new List<Value2D<T>>(this);
        foreach (Value2D<T> val in vals)
        {
            if (!types.Contains(val.val))
            {
                Remove(val);
            }
        }
    }
    #endregion

    #region Put All
    public void PutAll(Container2D<T> rhs)
    {
        foreach (Value2D<T> val in rhs)
        {
            this[val] = val.val;
        }
    }
    public void PutAll(Container2D<T> rhs, Point shift)
    {
        PutAll(rhs, shift.x, shift.y);
    }
    public void PutAll(Container2D<T> rhs, int xShift, int yShift)
    {
        foreach (Value2D<T> val in rhs)
        {
            this[val.x + xShift, val.y + yShift] = val.val;
        }
    }
    #endregion

    public bool Intersects(Container2D<T> rhs)
    {
        Bounding bounds = Bounding;
        Bounding rhsBounds = rhs.Bounding;
        Bounding intersect = bounds.IntersectBounds(rhsBounds);
        if (!intersect.IsValid()) return false;

        for (int y = intersect.YMin; y < intersect.YMax; y++)
            for (int x = intersect.XMin; x < intersect.XMax; x++)
                if (Contains(x, y) && rhs.Contains(x, y)) return true;
        return false;
    }

    public bool Intersects(IEnumerable<Container2D<T>> options, out Container2D<T> intersect)
    {
        foreach (Container2D<T> c in options)
        {
            if (Intersects(c))
            {
                intersect = c;
                return true;
            }
        }
        intersect = null;
        return false;
    }

    #region Logging
    public virtual List<string> ToRowStrings()
    {
        return Array.ToRowStrings();
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
            BigBoss.Debug.w(log, "Bounds: " + Bounding.ToString());
            BigBoss.Debug.printFooter(log, ToString());
        }
    }
    #endregion

    public static void Smallest<Z>(Z obj1, Z obj2, out Z smallest, out Z largest) where Z : Container2D<T>
    {
        if (obj1.Bounding.Area < obj2.Bounding.Area)
        {
            smallest = obj1;
            largest = obj2;
            return;
        }
        smallest = obj2;
        largest = obj1;
    }

    public Value2D<T> Random(System.Random random, bool take = false)
    {
        List<Value2D<T>> list = Random(random, 1, 0, take);
        if (list.Count == 0) return null;
        return list[0];
    }

    public virtual List<Value2D<T>> Random(System.Random random, int amount, int distance = 0, bool take = false)
    {
        throw new NotImplementedException();
    }

    public abstract IEnumerator<Value2D<T>> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
