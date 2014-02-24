using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MultiMap<T> : Container2D<T>
{
    bool _validCount = false;
    int _count = 0;
    Bounding _bounding = null;
    Array2D<T> _arr = null;
    protected Dictionary<int, Dictionary<int, T>> multimap = new Dictionary<int, Dictionary<int, T>>();
    public Dictionary<int, Dictionary<int, T>>.KeyCollection Keys { get { return multimap.Keys; } }
    public Dictionary<int, Dictionary<int, T>>.ValueCollection Values { get { return multimap.Values; } }

    #region Ctors
    public MultiMap()
    {
    }

    public MultiMap(Container2D<T> rhs)
        : base(rhs)
    {
    }

    public MultiMap(Container2D<T> rhs, Point shift)
        : base(rhs, shift)
    {
    }

    public MultiMap(Container2D<T> rhs, int xShift, int yShift)
        : base(rhs, xShift, yShift)
    {
    }
    #endregion

    public override T this[int x, int y]
    {
        get
        {
            Dictionary<int, T> row;
            if (multimap.TryGetValue(y, out row))
            {
                T val;
                if (row.TryGetValue(x, out val))
                {
                    return val;
                }
            }
            return default(T);
        }
        set
        {
            _bounding = null;
            _arr = null;
            _validCount = false;
            Dictionary<int, T> row;
            if (!multimap.TryGetValue(y, out row))
            {
                row = new Dictionary<int, T>();
                multimap[y] = row;
            }
            row[x] = value;
        }
    }

    public override bool TryGetValue(int x, int y, out T val)
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

    public override int Count
    {
        get
        {
            if (!_validCount)
            {
                _count = 0;
                foreach (Dictionary<int, T> row in multimap.Values)
                    _count += row.Count;
                _validCount = true;
            }
            return _count;
        }
    }

    public override Bounding Bounding
    {
        get
        {
            if (_bounding == null)
            {
                _bounding = new Bounding();
                foreach (KeyValuePair<int, Dictionary<int, T>> row in multimap)
                {
                    foreach (int x in row.Value.Keys)
                    {
                        _bounding.Absorb(x, row.Key);
                    }
                }
            }
            return new Bounding(_bounding);
        }
    }

    public override Array2D<T> Array
    {
        get
        {
            if (_arr != null) return _arr;
            _arr = new Array2D<T>(Bounding);
            foreach (KeyValuePair<int, Dictionary<int, T>> row in multimap)
            {
                foreach (KeyValuePair<int, T> val in row.Value)
                {
                    _arr[val.Key, row.Key] = val.Value;
                }
            }
            return _arr;
        }
    }

    public override bool Contains(int x, int y)
    {
        Dictionary<int, T> row;
        if (multimap.TryGetValue(y, out row))
        {
            return row.ContainsKey(x);
        }
        return false;
    }

    #region GetSet

    public override bool InRange(int x, int y)
    {
        return true;
    }

    public override bool Remove(int x, int y)
    {
        Dictionary<int, T> row;
        if (multimap.TryGetValue(y, out row))
        {
            if (row.Remove(x))
            {
                _validCount = false;
                _bounding = null;
                _arr = null;
                return true;
            }
        }
        return false;
    }

    public Value2D<T> RandomValue(System.Random rand)
    {
        if (Count != 0)
        {
            int pick = rand.Next(Count);
            return GetNth(pick);
        }
        return null;
    }
    #endregion

    public override List<Value2D<T>> GetRandom(System.Random random, int amount, int distance = 0, bool take = false)
    {
        MultiMap<T> removed = new MultiMap<T>();
        List<Value2D<T>> list = new List<Value2D<T>>();
        if (distance < 0) distance = 0;
        for (int i = 0; i < amount; i++)
        {
            Value2D<T> g = RandomValue(random);
            if (g == null) return list;
            if (distance > 0)
            {
                for (int yCur = g.y - distance; yCur <= g.y + distance; yCur++)
                {
                    for (int xCur = g.x - distance; xCur <= g.x + distance; xCur++)
                    {
                        removed[g] = g.val;
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

    public override IEnumerable<T> GetEnumerateValues()
    {
        foreach (KeyValuePair<int, Dictionary<int, T>> row in multimap)
        {
            foreach (KeyValuePair<int, T> val in row.Value)
            {
                yield return val.Value;
            }
        }
    }

    public override bool DrawAll(DrawAction<T> call)
    {
        foreach (KeyValuePair<int, Dictionary<int, T>> row in multimap)
        {
            foreach (KeyValuePair<int, T> val in row.Value)
            {
                if (!call(this, val.Key, row.Key)) return false;
            }
        }
        return true;
    }
    #endregion

    public override void Clear()
    {
        _validCount = false;
        _bounding = null;
        _arr = null;
        multimap.Clear();
    }

    public override Array2DRaw<T> RawArray(out Point shift)
    {
        return Array.RawArray(out shift);
    }

    public override void Shift(int x, int y)
    {
        if (x == 0)
        { // Just vertical
            Dictionary<int, Dictionary<int, T>> multimapRhs = new Dictionary<int, Dictionary<int, T>>();
            foreach (KeyValuePair<int, Dictionary<int, T>> row in multimap)
            {
                multimapRhs[row.Key + y] = row.Value;
            }
            multimap = multimapRhs;
        }
        else
        {
            MultiMap<T> rhs = new MultiMap<T>(this, x, y);
            this.multimap = rhs.multimap;
        }
    }
}
