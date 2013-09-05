using System;
using XML;

public struct FloatValue : Field
{
    float f;

    public FloatValue(float f)
    {
        this.f = f;
    }

    public static implicit operator float(FloatValue f)
    {
        return f.f;
    }

    public static implicit operator FloatValue(float f)
    {
        return new FloatValue(f);
    }

    public void parseXML(XMLNode x, string name)
    {
        f = XMLNifty.SelectFloat(x, name);
    }
}