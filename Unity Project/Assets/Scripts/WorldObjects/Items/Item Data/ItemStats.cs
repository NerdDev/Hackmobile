using System;
using XML;

public class ItemStats : IXmlParsable
{
    protected int cost;
    public int Cost
    {
        get { return cost; }
        set { this.cost = value; }
    }
    protected int weight;
    public int Weight
    {
        get { return weight; }
        set { this.weight = value; }
    }
    protected int nutrition;
    public int Nutrition
    {
        get { return nutrition; }
        set { this.nutrition = value; }
    }
    protected Size size;
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

    public int GetHash()
    {
        int hash = 5;
        hash += Weight.GetHashCode() * 3;
        hash += Nutrition.GetHashCode() * 5;
        hash += Size.GetHashCode() * 7;
        hash += Cost.GetHashCode() * 11;
        return hash;
    }
}
