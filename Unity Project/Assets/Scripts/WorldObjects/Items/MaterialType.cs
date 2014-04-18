using System;
using System.Collections.Generic;

public class MaterialType : IXmlParsable, INamed
{
    public string Name { get; set; }
    public bool Oxidizes;
    public bool Burns;
    public int Hardness;
    public int MeltingPoint;

    private float[] density = new float[3];
    public float Density;

    public void ParseXML(XMLNode x)
    {
        this.Name = x.SelectString("name");
        this.Hardness = x.SelectInt("hardness");
        this.Burns = x.SelectBool("burns");
        this.Oxidizes = x.SelectBool("oxidizes");

        XMLNode densityNode = x.Select("density");
        density[0] = densityNode.SelectFloat("min");
        density[2] = densityNode.SelectFloat("max");
        density[1] = (density[0] + density[2]) / 2;
        this.Density = density[Probability.Rand.Next(3)];

        this.MeltingPoint = x.SelectInt("meltingpoint");
    }
}
