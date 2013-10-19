using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class DictionaryExt
{
    public static V GetCreate<K, V>(this Dictionary<K, V> dict, K key)
    {
        V v;
        if (!dict.TryGetValue(key, out v))
        {
            v = (V) Activator.CreateInstance(typeof(V));
            dict.Add(key, v);
        }
        return v;
    }
}
