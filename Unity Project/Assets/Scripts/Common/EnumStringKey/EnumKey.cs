using UnityEngine;
using System.Collections;
using System;

public class EnumKey<T> : ESKey<T> where T : struct, IConvertible
{
    T e;

    public EnumKey(T e)
    {
        this.e = e;
    }

    public override int GetHashCode()
    {
        return e.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (!(obj is EnumKey<T>))
            return false;

        EnumKey<T> rhs = (EnumKey<T>)obj;

        return e.Equals(rhs.e);
    }
}
