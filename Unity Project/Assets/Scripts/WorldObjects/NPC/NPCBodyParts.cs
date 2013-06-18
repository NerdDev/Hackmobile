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
        int numOfParts;
        int.TryParse(x.select("arms").getText(), out numOfParts);
        Arms = numOfParts;
        int.TryParse(x.select("legs").getText(), out numOfParts);
        Legs = numOfParts;
        int.TryParse(x.select("heads").getText(), out numOfParts);
        Heads = numOfParts;
    }
}