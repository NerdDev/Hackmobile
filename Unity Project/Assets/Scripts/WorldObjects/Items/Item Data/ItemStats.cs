using System;
using XML;

public class ItemStats
{
    private int cost;
    public int Cost
    {
        get { return cost; }
        set { this.cost = value; }
    }
    private int weight;
    public int Weight
    {
        get { return weight; }
        set { this.weight = value; }
    }
    private int nutrition;
    public int Nutrition
    {
        get { return nutrition; }
        set { this.nutrition = value; }
    }
    private Size size;
    public Size Size
    {
        get { return size; }
        set { this.size = value; }
    }

    public void parseXML(XMLNode xnode)
    {
        //Assignation
        XMLNode x = XMLNifty.select(xnode, "stats");

        //Variables
        this.Weight = x.SelectInt("weight");
        this.Cost = x.SelectInt("cost");
        this.Nutrition = x.SelectInt("nutrition");
        this.Size = x.SelectEnum<Size>("equipsize");
    }

    public void setNull()
    {
        this.Cost = 0;
        this.Weight = 0;
        this.nutrition = 0;
        this.Size = Size.NONE;
    }
}