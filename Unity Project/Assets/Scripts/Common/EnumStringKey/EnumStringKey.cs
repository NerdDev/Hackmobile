using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnumStringKey<T> where T : struct, IConvertible {

    public static EnumStringKey<T> Get(string s)
    {
        try
        {
            EnumKey<T> ekey = new EnumKey<T>((T)Enum.Parse(typeof(T), s, true));
            return ekey;
        }
        catch (ArgumentException)
        {
            return new StringKey<T>(s);
        }
    }

    public static EnumStringKey<T> Get(T e)
    {
        return new EnumKey<T>(e);
    }

    public EnumStringKey()
    {
    }

    static public implicit operator EnumStringKey<T>(T e)
    {
        return Get(e);
    }

    static public implicit operator EnumStringKey<T>(string s)
    {
        return Get(s);
    }
}
