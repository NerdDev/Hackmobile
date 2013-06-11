using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MultiMap<T> {
    SortedDictionary<int, SortedDictionary<int, T>> multimap = new SortedDictionary<int, SortedDictionary<int, T>>();

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

    public List<SortedDictionary<int, T>> getRows()
    {
        return new List<SortedDictionary<int, T>>(multimap.Values);
    }

    public void put(T val, int x, int y)
    {
        SortedDictionary<int, T> row = GetRowCreate(y);
        row[x] = val;
    }

    public void putRow(T t, int xl, int xr, int y)
    {
        SortedDictionary<int, T> row = GetRowCreate(y);
        for (; xl <= xr; xl++)
        {
            row.Add(xl, t);
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

    public void putAll(MultiMap<T> rhs)
    {
        foreach (int y in rhs.multimap.Keys)
        {
            SortedDictionary<int, T> row = multimap[y];
            foreach (KeyValuePair<int, T> pair in rhs.multimap[y])
            {
                row.Add(pair.Key, pair.Value);
            }
        }
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
                    ret.rangeMin = ys.Current;
                }
                ret.rangeMax = ys.Current;
            }
        }
        throw new NotImplementedException();
    }

}
