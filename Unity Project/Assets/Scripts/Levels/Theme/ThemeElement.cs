using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ThemeElement : MonoBehaviour
{
    public GameObject GO { get { return gameObject; } }
    private Bounds _bounds;
    public Bounds Bounds 
    {  
        get
        {
            if (_bounds == null)
            {
                _bounds = new Bounds();
                foreach (Renderer render in GetComponentsInChildren<Renderer>(true))
                {
                    Bounds.Encapsulate(render.bounds);
                }
            }
            return _bounds;
        }
    }
    public byte GridWidth = 1;
    public byte GridHeight = 1;
}

