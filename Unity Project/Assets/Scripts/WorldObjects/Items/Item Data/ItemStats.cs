using System;
using XML;

public class ItemStats : FieldContainerClass
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

    public override void SetParams()
    {
        Weight = map.Add<Integer>("weight");
        Cost = map.Add<Integer>("cost");
        Nutrition = map.Add<Integer>("nutrition");
        Size = map.Add<EnumField<Size>>("equipsize");
    }
}
