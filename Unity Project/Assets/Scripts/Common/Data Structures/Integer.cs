using System;
using XML;

public struct Integer : Field
{
    int i;

    public Integer(int i)
    {
        this.i = i;
    }

    public static implicit operator int(Integer i)
    {
        return i.i;
    }

    public static implicit operator Integer(int i)
    {
        return new Integer(i);
    }

    public void parseXML(XMLNode x, string name)
    {
        this.i = XMLNifty.SelectInt(x, name);
    }
}