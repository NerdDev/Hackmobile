using System;
using System.Collections.Generic;
using XML;

public class MaterialType : FieldContainer
{
    public FieldMap map;

    public string Name { get; set; }
    public bool Oxidizes { get; set; }
    public bool Burns { get; set; }
    public int Hardness { get; set;}
    public int MeltingPoint { get; set; }
    public float Density { get; set; }

    public void SetParams()
    {
        this.Name = map.Add<String>("name");
        this.Hardness = map.Add<Integer>("hardness");
        this.Burns = map.Add<BoolValue>("burns");
        this.Oxidizes = map.Add<BoolValue>("oxidizes");
        this.Density = map.Add<Float>("density");
        this.MeltingPoint = map.Add<Integer>("meltingpoint");
    }

    public void parseXML(XMLNode x)
    {
        map = new FieldMap(x);
        this.SetParams();
    }

    public virtual void setNull()
    {
        this.parseXML(new XMLNode());
    }
}
