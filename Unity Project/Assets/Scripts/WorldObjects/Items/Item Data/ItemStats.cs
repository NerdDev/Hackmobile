using System;
using XML;

public class ItemStats : IXmlParsable
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

    public void ParseXML(XMLNode x)
    {
        Weight = x.SelectInt("weight");
        Cost = x.SelectInt("cost");
        Nutrition = x.SelectInt("nutrition");
        Size = x.SelectEnum<Size>("equipsize");
    }
}
