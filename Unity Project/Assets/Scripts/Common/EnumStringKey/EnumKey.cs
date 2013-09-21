using UnityEngine;
using System.Collections;
using System;

public class EnumKey<T> : ESKey<T> where T : struct, IComparable, IConvertible
{
    public T Key { get; protected set; }

    public EnumKey(T e)
    {
        this.Key = e;
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (!(obj is EnumKey<T>))
            return false;

        EnumKey<T> rhs = (EnumKey<T>)obj;

        return Key.Equals(rhs.Key);
    }
}
