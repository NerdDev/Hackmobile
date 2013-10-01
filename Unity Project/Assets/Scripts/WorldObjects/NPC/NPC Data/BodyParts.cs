using System;
using XML;

public class BodyParts : FieldContainerClass
{
    public int Arms { get; set; }
    public int Legs { get; set; }
    public int Heads { get; set; }

    public BodyParts()
    {
        Arms = 2;
        Legs = 2;
        Heads = 1;
    }

    public override void SetParams()
    {
        base.SetParams();
        Arms = map.Add<Integer>("arms");
        Legs = map.Add<Integer>("legs");
        Heads = map.Add<Integer>("heads");
    }
}
