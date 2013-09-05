using System;
using XML;

public struct IntegerValue : Field
{
    int i;

    public IntegerValue(int i)
    {
        this.i = i;
    }

    public static implicit operator int(IntegerValue i)
    {
        return i.i;
    }

    public static implicit operator IntegerValue(int i)
    {
        return new IntegerValue(i);
    }

    public void parseXML(XMLNode x, string name)
    {
        this.i = XMLNifty.SelectInt(x, name);
    }
}