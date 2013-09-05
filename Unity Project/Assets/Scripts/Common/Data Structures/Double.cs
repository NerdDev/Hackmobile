using System;
using XML;

public struct Double : Field
{
    double d;

    public Double(double d)
    {
        this.d = d;
    }

    public static implicit operator double(Double d)
    {
        return d.d;
    }

    public static implicit operator Double(double d)
    {
        return new Double(d);
    }

    public void parseXML(XMLNode x, string name)
    {
        this.d = XMLNifty.SelectDouble(x, name);
    }
}