using UnityEngine;
using System.Collections;
using System;

public class StringKey<T> : ESKey<T> where T : struct, IComparable, IConvertible
{
    public string Key { get; protected set; }

    public StringKey(string key)
    {
        Key = key.ToUpper();
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (!(obj is StringKey<T>))
            return false;

        StringKey<T> rhs = (StringKey<T>)obj;

        return Key.Equals(rhs.Key);
    }
}
