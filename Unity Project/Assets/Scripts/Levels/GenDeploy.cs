using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GenDeploy : ITransform
{
    public ThemeElement Element;
    public List<GenSpace> Spaces;
    public char PrintChar = 'D';
    public float Rotation { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    
    public GenDeploy(ThemeElement element)
    {
        Element = element;
        Spaces = new List<GenSpace>(Element.GridWidth * Element.GridHeight);
    }

    public void AddSpace(GenSpace space)
    {
        space.AddDeploy(this);
    }
}

