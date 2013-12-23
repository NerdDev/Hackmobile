using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MultiMap<T> : Container2D<T>
{
    int _count = 0;
    public int Count { get { return _count; } }
    public bool IsEmpty { get { return Count == 0; } }
    protected Dictionary<int, Dictionary<int, T>> multimap = new Dictionary<int, Dictionary<int, T>>();
    public Dictionary<int, Dictionary<int, T>>.KeyCollection Keys { get { return multimap.Keys; } }
    public Dictionary<int, Dictionary<int, T>>.ValueCollection Values { get { return multimap.Values; } }

    #region Ctors
    public MultiMap()
    {
    }

    public MultiMap(MultiMap<T> rhs)
        : this()
    {
        PutAll(rhs);
    }

    public MultiMap(MultiMap<T> rhs, Point shift)
        : this()
    {
        PutAll(rhs, shift);
    }

    public MultiMap(MultiMap<T> rhs, int xShift, int yShift)
        : this()
    {
        PutAll(rhs, xShift, yShift);
    }
    #endregion

    #region GetSet
    protected override T Get(int x, int y)
    {
        T val;
        TryGetValue(x, y, out val);
        return val;
    }

    public bool Contains(int x, int y)
    {
        Dictionary<int, T> row;
        if (GetRow(y, out row))
        {
            return row.ContainsKey(x);
        }
        return false;
    }

    public bool GetRow(int y, out Dictionary<int, T> val)
    {
        bool ret = multimap.TryGetValue(y, out val);
        return ret;
    }

    public override bool InRange(int x, int y)
    {
        return true;
    }

    Dictionary<int, T> GetRowCreate(int y)
    {
        Dictionary<int, T> row;
        if (!multimap.TryGetValue(y, out row))
        {
            row = new Dictionary<int, T>();
            multimap[y] = row;
        }
        return row;
    }

    public override void Put(T val, int x, int y)
    {
        PutInternal(val, x, y);
    }

    protected override void PutInternal(T val, int x, int y)
    {
        Dictionary<int, T> row = GetRowCreate(y);
        Put(row, val, x);
    }

    void Put(Dictionary<int, T> row, T val, int x)
    {
        if (!row.ContainsKey(x))
            _count++;
        row[x] = val;
    }

    public void Remove(int x, int y)
    {
        Dictionary<int, T> row;
        if (GetRow(y, out row))
        {
            if (row.Remove(x))
                _count--;
        }
    }

    public void Remove(Value2D<T> val)
    {
        Remove(val.x, val.y);
    }

    public void Remove(int x, int y, int width)
    {
        for (int yCur = y - width; yCur <= y + width; yCur++)
        {
            for (int xCur = x - width; xCur <= x + width; xCur++)
            {
                Remove(xCur, yCur);
            }
        }
    }

    public void PutAll(MultiMap<T> rhs)
    {
        foreach (Value2D<T> val in rhs)
        {
            this[val] = val.val;
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
        Dictionary<int, T> row = GetRowCreate(y);
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

    public override void PutCol(T t, int y1, int y2, int x)
    {
        for (; y1 <= y2; y1++)
        {
            Put(t, x, y1);
        }
    }

    public bool TryGetValue(int x, int y, out T val)
    {
        Dictionary<int, T> row;
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
        foreach (KeyValuePair<int, Dictionary<int, T>> row in multimap)
        {
            foreach (KeyValuePair<int, T> val in row.Value)
            {
                bounds.Absorb(val.Key, row.Key);
            }
        }
        return bounds;
    }

    public override T[,] GetArr()
    {
        Bounding bounds = GetBounding();
        T[,] arr = new T[bounds.Height + 1, bounds.Width + 1];
        foreach (KeyValuePair<int, Dictionary<int, T>> row in multimap)
        {
            foreach (KeyValuePair<int, T> val in row.Value)
            {
                arr[row.Key - bounds.YMin, val.Key - bounds.XMin] = val.Value;
            }
        }
        return arr;
    }

    public Value2D<T> Random(System.Random rand)
    {
        if (Count != 0)
        {
            int pick = rand.Next(Count);
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

    public override List<Value2D<T>> Random(System.Random random, int amount, int distance = 0, bool take = false)
    {
        MultiMap<T> removed = new MultiMap<T>();
        List<Value2D<T>> list = new List<Value2D<T>>();
        if (distance < 0) distance = 0;
        for (int i = 0; i < amount; i++)
        {
            Value2D<T> g = Random(random);
            if (g == null) return list;
            if (distance > 0)
            {
                for (int yCur = g.y - distance; yCur <= g.y + distance; yCur++)
                {
                    for (int xCur = g.x - distance; xCur <= g.x + distance; xCur++)
                    {
                        removed.Put(g);
                        Remove(xCur, yCur);
                    }
                }
            }
            Remove(g.x, g.y, distance - 1);
            list.Add(g);
        }
        if (take)
        {
            foreach (Value2D<T> val in list)
                removed.Remove(val);
        }
        this.PutAll(removed);
        return list;
    }

    #region Iteration
    public override IEnumerator<Value2D<T>> GetEnumerator()
    {
        foreach (KeyValuePair<int, Dictionary<int, T>> row in multimap)
        {
            foreach (KeyValuePair<int, T> val in row.Value)
            {
                var ret = new Value2D<T>(val.Key, row.Key, val.Value);
                yield return ret;
            }
        }
    }
    #endregion
}
