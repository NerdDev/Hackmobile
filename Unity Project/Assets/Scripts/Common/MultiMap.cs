using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiMap<T> {
    Dictionary<int, Dictionary<int, T>> multimap = new Dictionary<int, Dictionary<int, T>>();

    public T get(int x, int y)
    {
        T val;
        TryGetValue(x, y, out val);
        return val;
    }

    Dictionary<int, T> get(int x)
    {
        return multimap[x];
    }

    public void put(T val, int x, int y)
    {
        Dictionary<int, T> col;
        if (!multimap.TryGetValue(x, out col))
        {
            col = new Dictionary<int, T>();
            multimap[x] = col;
        }
        col[y] = val;
    }

    public bool TryGetValue(int x, int y, out T val)
    {
        Dictionary<int, T> col;
        if (multimap.TryGetValue(x, out col))
        {
            if (col.TryGetValue(y, out val))
            {
                return true;
            }
        }
        val = default(T);
        return false;
    }

    public void putAll(MultiMap<T> rhs)
    {
        foreach (int x in rhs.multimap.Keys)
        {
            Dictionary<int, T> col = get(x);
            foreach (KeyValuePair<int, T> pair in rhs.get(x))
            {
                col.Add(pair.Key, pair.Value);
            }
        }
    }
}
