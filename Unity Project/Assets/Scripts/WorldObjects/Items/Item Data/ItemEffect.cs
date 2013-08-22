using System;
using XML;

public class ItemEffect
{
    public Properties prop;
    public int turns;
    public int strength;

    public void parseXML(XMLNode x)
    {
        prop = XMLNifty.SelectEnum<Properties>(x, "effect");
        turns = XMLNifty.SelectInt(x, "turns");
        strength = XMLNifty.SelectInt(x, "strength");
    }
}