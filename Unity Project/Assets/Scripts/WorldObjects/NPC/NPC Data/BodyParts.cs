using System;
using XML;

public class BodyParts : FieldContainerClass
{
    private int arms;
    public int Arms { get { return arms; } set { this.arms = value; } }
    private int legs;
    public int Legs { get { return legs; } set { this.legs = value; } }
    private int heads;
    public int Heads { get { return heads; } set { this.heads = value; } }

    public override void SetParams()
    {
        base.SetParams();
        Arms = map.Add<Integer>("arms");
        Legs = map.Add<Integer>("legs");
        Heads = map.Add<Integer>("heads");
    }
}