using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MultiMap<T> : Container2D<T>, IEnumerable<Value2D<T>> {

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
        PutAll(rhs);
    }

    public MultiMap(MultiMap<T> rhs, Point shift) : this()
    {
        PutAll(rhs, shift);
    }

    public MultiMap(MultiMap<T> rhs, int xShift, int yShift) : this()
    {
        PutAll(rhs, xShift, yShift);
    }

    protected virtual void setComparator() 
    {
    }
    #endregion

    #region GetSet
    public override T Get(int x, int y)
    {
        T val;
        TryGetValue(x, y, out val);
        return val;
    }

    public bool Contains(int x, int y)
    {
        SortedDictionary<int, T> row;
        if (GetRow(y, out row))
        {
            return row.ContainsKey(x);
        }
        return false;
    }

    public bool GetRow(int y, out SortedDictionary<int, T> val)
    {
        bool ret = multimap.TryGetValue(y, out val);
        return ret;
    }

    public override bool InRange(int x, int y)
    {
        return true;
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

    public override void Put(T val, int x, int y)
    {
        PutInternal(val, x, y);
    }

    public void Put(Value2D<T> val)
    {
        Put(val.val, val.x, val.y);
    }

    protected override void PutInternal(T val, int x, int y)
    {
        SortedDictionary<int, T> row = GetRowCreate(y);
        Put(row, val, x);
    }

    void Put(SortedDictionary<int, T> row, T val, int x)
    {
        if (!row.ContainsKey(x)    // If value doesn't already exist
            || comparator == null  // Or comparator is null
            || 1 == comparator.compare(val, row[x])) // Or if new value is greater
        {
            row[x] = val;
        }
    }

    public void Remove(int x, int y)
    {
        SortedDictionary<int, T> row;
        if (GetRow(y, out row))
        {
            row.Remove(x);
        }
    }

    public void Remove(Value2D<T> val)
    {
        Remove(val.x, val.y);
    }

    public void PutAll(MultiMap<T> rhs)
    {
        foreach (Value2D<T> val in rhs)
        {
            Put(val);
        }
    }

    public void PutAll(MultiMap<T> rhs, Point shift)
    {
        PutAll(rhs, shift.x, shift.y);
    }

    public void PutAll(MultiMap<T> rhs, int xShift, int yShift)
    {
        foreach (Value2D<T> val in rhs)
        {
            Put(val.val, val.x + xShift, val.y + yShift);
        }
    }

    public void PutRow(T t, int xl, int xr, int y)
    {
        SortedDictionary<int, T> row = GetRowCreate(y);
        for (; xl <= xr; xl++)
        {
            row[xl] = t;
        }
	}

    public void RemoveAll(MultiMap<T> rhs)
    {
        foreach (Value2D<T> val in rhs)
        {
            Remove(val);
        }
    }

    public override void PutCol(T t, int y1, int y2, int x)
    {
        for (; y1 <= y2; y1++)
        {
            Put(t, x, y1);
        }
    }

    public void PutSquare(T t, int xl, int xr, int yb, int yt)
    {
        for (; yb <= yt; yb++)
        {
            PutRow(t, xl, xr, yb);
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

    public override Bounding GetBounding()
    {
		Bounding bounds = new Bounding();
        foreach (KeyValuePair<int, SortedDictionary<int, T>> row in multimap)
        {
            foreach (KeyValuePair<int, T> val in row.Value)
            {
                bounds.absorb(val.Key, row.Key);
            }
        }
		return bounds;
    }

    public override T[,] GetArr()
    {
        Bounding bounds = GetBounding();
        T[,] arr = new T[bounds.height + 1, bounds.width + 1];
        foreach (KeyValuePair<int, SortedDictionary<int, T>> row in multimap)
        {
            foreach (KeyValuePair<int, T> val in row.Value)
            {
                arr[row.Key - bounds.yMin, val.Key - bounds.xMin] = val.Value;
            }
        }
		return arr;
    }

    public int Count()
    {
        int ret = multimap.Count;
        foreach (SortedDictionary<int, T> row in multimap.Values)
        {
            ret += row.Count;
        }
        return ret;
    }

    public bool isEmpty()
    {
        return Count() == 0;
    }

    public Value2D<T> RandomValue(System.Random rand)
    {
        int count = Count();
        if (count != 0)
        {
            int pick = rand.Next(count);
            return GetNth(pick);
        }
        return null;
    }

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
    #endregion

    #region Iteration
    public IEnumerator<Value2D<T>> GetEnumerator()
    {
        foreach (KeyValuePair<int, SortedDictionary<int, T>> row in multimap)
        {
            foreach (KeyValuePair<int, T> val in row.Value)
            {
                Value2D<T> ret = new Value2D<T>(val.Key, row.Key, val.Value);
                yield return ret;
            }
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
    #endregion
}
