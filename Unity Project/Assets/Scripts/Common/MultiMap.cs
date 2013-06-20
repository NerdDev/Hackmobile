using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MultiMap<T> : IEnumerable<KeyValuePair<int, SortedDictionary<int, T>>> {

    SortedDictionary<int, SortedDictionary<int, T>> multimap = new SortedDictionary<int, SortedDictionary<int, T>>();
    public SortedDictionary<int, SortedDictionary<int, T>>.KeyCollection Keys { get { return multimap.Keys; } }
    public SortedDictionary<int, SortedDictionary<int, T>>.ValueCollection Values { get { return multimap.Values; } }
    public Comparator<T> comparator { get; set; }

    #region Ctors
    public MultiMap()
    {
        setComparator();
    }

    public MultiMap(MultiMap<T> rhs) : this()
    {
        putAll(rhs);
    }

    public MultiMap(MultiMap<T> rhs, Point shift) : this()
    {
        putAll(rhs, shift);
    }

    public MultiMap(MultiMap<T> rhs, int xShift, int yShift) : this()
    {
        putAll(rhs, xShift, yShift);
    }

    protected virtual void setComparator() 
    {
    }
    #endregion

    #region GetSet
    public T get(int x, int y)
    {
        T val;
        TryGetValue(x, y, out val);
        return val;
    }

    public void getRow(int y, out SortedDictionary<int, T> val)
    {
        multimap.TryGetValue(y, out val);
    }

    SortedDictionary<int, T> GetRowCreate(int y)
    {
        SortedDictionary<int, T> row;
        if (!multimap.TryGetValue(y, out row))
        {
            row = new SortedDictionary<int, T>();
            multimap[y] = row;
        }
        return row;
    }

    public void put(T val, int x, int y)
    {
        SortedDictionary<int, T> row = GetRowCreate(y);
        put(row, val, x);
    }

    void put(SortedDictionary<int, T> row, T val, int x)
    {
        if (!row.ContainsKey(x)    // If value doesn't already exist
            || comparator == null  // Or comparator is null
            || 1 == comparator.compare(val, row[x])) // Or if new value is greater
        {
            row[x] = val;
        }
    }

    public void putAll(MultiMap<T> rhs)
    {
        foreach (KeyValuePair<int, SortedDictionary<int, T>> rowRhs in rhs)
        {
            SortedDictionary<int, T> row = GetRowCreate(rowRhs.Key);
            foreach (KeyValuePair<int, T> rhsItem in rowRhs.Value)
            {
                put(row, rhsItem.Value, rhsItem.Key);
            }
        }
    }

    public void putAll(MultiMap<T> rhs, Point shift)
    {
        putAll(rhs, shift.x, shift.y);
    }

    public void putAll(MultiMap<T> rhs, int xShift, int yShift)
    {
        foreach (KeyValuePair<int, SortedDictionary<int, T>> rowRhs in rhs)
        {
            SortedDictionary<int, T> row = GetRowCreate(rowRhs.Key + yShift);
            foreach (KeyValuePair<int, T> rhsItem in rowRhs.Value)
            {
                row[rhsItem.Key + xShift] = rhsItem.Value;
            }
        }
    }

    public void putRow(T t, int xl, int xr, int y)
    {
        SortedDictionary<int, T> row = GetRowCreate(y);
        for (; xl <= xr; xl++)
        {
            row[xl] = t;
        }
	}

    public void putCol(T t, int y1, int y2, int x)
    {
        for (; y1 <= y2; y1++)
        {
            put(t, x, y1);
        }
    }

    public void putSquare(T t, int xl, int xr, int yb, int yt)
    {
        for (; yb <= yt; yb++)
        {
            putRow(t, xl, xr, yb);
        }
    }

    public bool TryGetValue(int x, int y, out T val)
    {
        SortedDictionary<int, T> row;
        if (multimap.TryGetValue(y, out row))
        {
            if (row.TryGetValue(x, out val))
            {
                return true;
            }
        }
        val = default(T);
        return false;
    }

    public Bounding getBounding()
    {
        Bounding ret = new Bounding();
        if (multimap.Count > 0)
        {
            IEnumerator<int> ys = multimap.Keys.GetEnumerator();
            bool first = true;
            while (ys.MoveNext())
            {
                if (first)
                {
                    first = false;
                    ret.yMin = ys.Current;
                }
                ret.yMax = ys.Current;
            }
        }
        throw new NotImplementedException();
    }
    #endregion

    #region Iteration
    public IEnumerator<KeyValuePair<int, SortedDictionary<int, T>>> GetEnumerator()
    {
        return multimap.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
    #endregion
}
