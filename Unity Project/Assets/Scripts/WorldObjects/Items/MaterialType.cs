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

    private double[] density = new double[3];
    public double Density { get; set; }

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
        this.Name = x.SelectString("name");
        this.Hardness = x.SelectInt("hardness");
        this.Burns = x.SelectBool("burns");
        this.Oxidizes = x.SelectBool("oxidizes");

        density[0] = x.select("density").SelectDouble("min");
        density[2] = x.select("density").SelectDouble("max");
        density[1] = (density[0] + density[2]) / 2;
        this.Density = density[Probability.getRandomInt(3)];

        this.MeltingPoint = x.SelectInt("meltingpoint");
    }
}