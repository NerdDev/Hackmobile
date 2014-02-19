using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GridDeploy
{
    public ThemeElement Element;
    public GameObject GO { get { return Element.GO; } }
    public float Rotation;
    public float X;
    public float Y;
    public float Z;

    public GridDeploy(ThemeElement element)
    {
        this.Element = element;
    }
}

