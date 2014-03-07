using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GridDeploy : ITransform
{
    public GameObject GO;
    public float Rotation { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public GridDeploy(GameObject go)
    {
        this.GO = go;
    }
}

