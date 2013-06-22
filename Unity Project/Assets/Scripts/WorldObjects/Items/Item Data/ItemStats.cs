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

    public void parseXML(XMLNode x)
    {
        this.Weight = Nifty.StringToInt(x.select("weight").getText());
        this.Cost = Nifty.StringToInt(x.select("cost").getText());
        this.Nutrition = Nifty.StringToInt(x.select("nutrition").getText());
        this.Size = (Size) Enum.Parse(size.GetType(), x.select("equipsize").getText(), true);
    }

    public void setNull()
    {
        this.Cost = 0;
        this.Weight = 0;
        this.nutrition = 0;
    }
}