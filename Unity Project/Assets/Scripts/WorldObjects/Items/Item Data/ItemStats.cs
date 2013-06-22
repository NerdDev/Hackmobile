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

    public void parseXML(XMLNode x)
    {
        this.Weight = Nifty.StringToInt(x.select("weight").getText());
        this.Cost = Nifty.StringToInt(x.select("cost").getText());
    }

    public void setData(ItemStats istats)
    {
        this.Cost = istats.Cost;
        this.Weight = istats.Weight;
        this.nutrition = istats.nutrition;
    }

    public void setNull()
    {
        this.Cost = 0;
        this.Weight = 0;
        this.nutrition = 0;
    }
}