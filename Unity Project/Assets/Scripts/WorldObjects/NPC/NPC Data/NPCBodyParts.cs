using System;
using XML;

public class NPCBodyParts
{
    private int arms;
    public int Arms { get { return arms; } set { this.arms = value; } }
    private int legs;
    public int Legs { get { return legs; } set { this.legs = value; } }
    private int heads;
    public int Heads { get { return heads; } set { this.heads = value; } }

    public void setData(NPCBodyParts nbp)
    {
        this.Arms = nbp.Arms;
        this.Legs = nbp.Legs;
        this.Heads = nbp.Heads;
    }

    public void parseXML(XMLNode x)
    {
        Arms = Nifty.StringToInt(x.select("arms").getText());
        Legs = Nifty.StringToInt(x.select("legs").getText());
        Heads = Nifty.StringToInt(x.select("heads").getText());
    }

    public void setNull()
    {
        this.Arms = 0;
        this.Legs = 0;
        this.Heads = 0;
    }
}