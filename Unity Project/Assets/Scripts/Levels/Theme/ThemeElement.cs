using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ThemeElement
{
    public GameObject GO;
    public Bounds Bounds;

    public ThemeElement(GameObject go)
    {
        GO = go;
        Bounds = new Bounds();
        foreach (Renderer render in go.GetComponentsInChildren<Renderer>(true))
        {
            Bounds.Encapsulate(render.bounds);
        }
    }
}

