using System;
using XML;

public class String : Field
{
    string s;

    public String()
    {
        s = "";
    }

    public String(string s)
    {
        this.s = s;
    }

    public static implicit operator string(String s)
    {
        return s.s;
    }

    public static implicit operator String(string s)
    {
        return new String(s);
    }

    public void parseXML(XMLNode x, string name)
    {
        if (x != null)
            s = x.SelectString(name);
        else
            s = "";
    }
}
