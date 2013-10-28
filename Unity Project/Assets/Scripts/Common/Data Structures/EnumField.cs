using System;
using XML;

public struct EnumField<T> : Field where T : IComparable, IConvertible
{
    public T e;

    public EnumField(EnumField<T> e)
    {
        this.e = (T) Enum.ToObject(typeof(T), Convert.ToInt32(e.e));
    }

    public static implicit operator T(EnumField<T> src)
    {
        return src.e != null ? (T)Enum.ToObject(typeof(T), Convert.ToInt32(src.e)) : (T)Enum.ToObject(typeof(T), 0);
    }

    public static implicit operator EnumField<T>(T src)
    {
        return new EnumField<T>(src);
    }

    public void parseXML(XMLNode x, string name)
    {
        if (x != null)
            e = x.SelectEnum<T>(name);
        else
            e = default(T);
    }
}
