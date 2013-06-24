using System;
using System.Collections.Generic;
using XML;

public class MaterialType
{
    public string Name { get; set; }
    public bool Oxidizes { get; set; }
    public bool Burns { get; set; }
    public int Hardness { get; set;}

    public void setNull()
    {
        Name = "null";
        Oxidizes = false;
        Burns = false;
        Hardness = 0;
    }

    public void parseXML(XMLNode x)
    {
        this.Name = x.SelectString("name");
        this.Hardness = x.SelectInt("hardness");
        this.Burns = x.SelectBool("burns");
        this.Oxidizes = x.SelectBool("oxidizes");
    }
}