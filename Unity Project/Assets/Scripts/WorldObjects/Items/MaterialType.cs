using System;
using System.Collections.Generic;

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
}