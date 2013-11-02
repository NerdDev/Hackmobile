using System;
using XML;

public struct BoolValue : Field
{
    bool b;

    public BoolValue(bool b)
    {
        this.b = b;
    }

    public static implicit operator bool(BoolValue b)
    {
        return b.b;
    }

    public static implicit operator BoolValue(bool b)
    {
        return new BoolValue(b);
    }

    public void parseXML(XMLNode x, string name)
    {
        if (x != null)
            b = x.SelectBool(name);
        else
            b = false;
    }


    public new string ToString()
    {
        return b.ToString();
    }
}
