using System;
using XML;

public class BodyParts : IXmlParsable
{
    public int Arms;
    public int Legs;
    public int Heads;

    public void ParseXML(XMLNode x)
    {
        Arms = x.SelectInt("arms");
        Legs = x.SelectInt("legs");
        Heads = x.SelectInt("heads");
    }
}
