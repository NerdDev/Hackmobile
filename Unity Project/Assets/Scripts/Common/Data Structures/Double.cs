using System;
using XML;

public struct DoubleValue : Field
{
    double d;

    public DoubleValue(double d)
    {
        this.d = d;
    }

    public static implicit operator double(DoubleValue d)
    {
        return d.d;
    }

    public static implicit operator DoubleValue(double d)
    {
        return new DoubleValue(d);
    }

    public void parseXML(XMLNode x, string name)
    {
        this.d = XMLNifty.SelectDouble(x, name);
    }
}