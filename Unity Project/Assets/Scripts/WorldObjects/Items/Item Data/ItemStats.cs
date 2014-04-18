using System;

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

    public EquipType EquipType { get; set; }
    public int NumberOfSlots { get; set; }
    public Damage damage { get; set; }

    public void ParseXML(XMLNode x)
    {
        Weight = x.SelectInt("weight");
        Cost = x.SelectInt("cost");
        Nutrition = x.SelectInt("nutrition");
        damage = x.Select<Damage>("damage");
        EquipType = x.SelectEnum<EquipType>("equiptype");
        NumberOfSlots = x.SelectInt("slots", 1);
    }

    public int GetHash()
    {
        int hash = 5;
        hash += Weight.GetHashCode() * 3;
        hash += Nutrition.GetHashCode() * 5;
        hash += Cost.GetHashCode() * 11;
        hash += EquipType.GetHashCode() * 17;
        hash += NumberOfSlots.GetHashCode() * 13;
        return hash;
    }
}
