using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ESKey<T> where T : struct, IComparable, IConvertible {

    public static ESKey<T> Get(string s)
    {
        T e;
        if (s.ToEnum<T>(out e))
        {
            EnumKey<T> ekey = new EnumKey<T>((T)Enum.Parse(typeof(T), s, true));
            return ekey;
        }
        else
        {
            return new StringKey<T>(s);
        }
    }

    public static ESKey<T> Get(T e)
    {
        return new EnumKey<T>(e);
    }

    public ESKey()
    {
    }

    static public implicit operator ESKey<T>(T e)
    {
        return Get(e);
    }

    static public implicit operator ESKey<T>(string s)
    {
        return Get(s);
    }
}
