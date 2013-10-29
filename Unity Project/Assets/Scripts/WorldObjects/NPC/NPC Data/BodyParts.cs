using System;
using XML;

public class BodyParts : IXmlParsable
{
    public int Arms { get; set; }
    public int Legs { get; set; }
    public int Heads { get; set; }

    public void ParseXML(XMLNode x)
    {
        Arms = x.SelectInt("arms");
        Legs = x.SelectInt("legs");
        Heads = x.SelectInt("heads");
    }
}
