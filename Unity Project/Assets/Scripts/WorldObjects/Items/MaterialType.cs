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

    public void parseXML(XMLNode m)
    {
        this.Name = m.select("name").getText();
        this.Hardness = Nifty.StringToInt(m.select("hardness").getText());
        this.Burns = Nifty.StringToBool(m.select("burns").getText());
        this.Oxidizes = Nifty.StringToBool(m.select("oxidizes").getText());
    }
}