using UnityEngine;
using System.Collections;
using System;

public class StringKey<T> : ESKey<T> where T : struct, IComparable, IConvertible
{
    string str;

    public StringKey(string key)
    {
        str = key.ToUpper();
    }

    public override int GetHashCode()
    {
        return str.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (!(obj is StringKey<T>))
            return false;

        StringKey<T> rhs = (StringKey<T>)obj;

        return str.Equals(rhs.str);
    }
}
