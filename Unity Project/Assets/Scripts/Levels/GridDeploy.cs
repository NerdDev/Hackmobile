using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GridDeploy : ITransform
{
    public GameObject GO;
    public float XRotation { get; set; }
    public float YRotation { get; set; }
    public float ZRotation { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float XScale { get; set; }
    public float YScale { get; set; }
    public float ZScale { get; set; }

    public GridDeploy(GameObject go)
    {
        this.GO = go;
        XScale = 1F;
        YScale = 1F;
        ZScale = 1F;
    }
}

