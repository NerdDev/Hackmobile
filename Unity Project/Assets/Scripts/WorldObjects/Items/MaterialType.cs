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
        Name = "null";
        Oxidizes = false;
        Burns = false;
        Hardness = 0;
        Density = 0;
    }

    public void parseXML(XMLNode x)
    {
        this.Name = XMLNifty.SelectString(x, "name");
        this.Hardness = XMLNifty.SelectInt(x, "hardness");
        this.Burns = XMLNifty.SelectBool(x, "burns");
        this.Oxidizes = XMLNifty.SelectBool(x, "oxidizes");

        XMLNode densityNode = XMLNifty.select(x, "density");
        density[0] = XMLNifty.SelectFloat(densityNode, "min");
        density[2] = XMLNifty.SelectFloat(densityNode, "max");
        density[1] = (density[0] + density[2]) / 2;
        this.Density = density[Probability.getRandomInt(3)];

        this.MeltingPoint = XMLNifty.SelectInt(x, "meltingpoint");
    }
}