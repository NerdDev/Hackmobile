using System;
using XML;

public class BodyParts
{
    private int arms;
    public int Arms { get { return arms; } set { this.arms = value; } }
    private int legs;
    public int Legs { get { return legs; } set { this.legs = value; } }
    private int heads;
    public int Heads { get { return heads; } set { this.heads = value; } }

    public void setData(BodyParts nbp)
    {
        this.Arms = nbp.Arms;
        this.Legs = nbp.Legs;
        this.Heads = nbp.Heads;
    }

    public void parseXML(XMLNode xnode)
    {
        //Assignation of node
        XMLNode x = XMLNifty.select(xnode, "bodyparts");

        //Variable parse
        Arms = XMLNifty.SelectInt(x, "arms");
        Legs = XMLNifty.SelectInt(x, "legs");
        Heads = XMLNifty.SelectInt(x, "heads");
    }

    public void setNull()
    {
        this.Arms = 0;
        this.Legs = 0;
        this.Heads = 0;
    }
}