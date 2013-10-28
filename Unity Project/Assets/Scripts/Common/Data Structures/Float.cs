using System;
using XML;

public struct Float : Field
{
    float f;

    public Float(float f)
    {
        this.f = f;
    }

    public static implicit operator float(Float f)
    {
        return f.f;
    }

    public static implicit operator Float(float f)
    {
        return new Float(f);
    }

    public void parseXML(XMLNode x, string name)
    {
        if (x != null)
            f = x.SelectFloat(name);
        else
            f = 0f;
    }
}
