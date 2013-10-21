using System;
using System.Collections.Generic;
using XML;

public class MaterialType
{
    public string Name { get; set; }
    public bool Oxidizes { get; set; }
    public bool Burns { get; set; }
    public int Hardness { get; set;}
    public int MeltingPoint { get; set; }

    private float[] density = new float[3];
    public float Density { get; set; }

    public void setNull()
    {
        this.parseXML(new XMLNode(null));
    }

    public void parseXML(XMLNode x)
    {
        this.Name = x.SelectString("name");
        this.Hardness = x.SelectInt("hardness");
        this.Burns = x.SelectBool("burns");
        this.Oxidizes = x.SelectBool("oxidizes");

        XMLNode densityNode = x.Select("density");
        density[0] = densityNode.SelectFloat("min");
        density[2] = densityNode.SelectFloat("max");
        density[1] = (density[0] + density[2]) / 2;
        this.Density = density[Probability.getRandomInt(3)];

        this.MeltingPoint = x.SelectInt("meltingpoint");
    }
}
