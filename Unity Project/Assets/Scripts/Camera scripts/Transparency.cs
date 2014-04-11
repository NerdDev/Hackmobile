using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Transparency : MonoBehaviour
{

    public Material original;
    public Material transparent;

    float timeTillEnd;
    public void Update()
    {
        if (timeTillEnd < Time.time)
        {
            End();
            Destroy(this);
        }
    }

    public void Extend(float extension)
    {
        timeTillEnd = Time.time + extension;
    }

    public void init(float initialTime, float transparency)
    {
        timeTillEnd = Time.time + initialTime;
        foreach (Renderer r in this.GetComponentsInChildren<Renderer>())
        {
            original = r.sharedMaterial;
            transparent = new Material(original);
            transparent.shader = BigBoss.Gooey.TransparentShader;
            Color temp = transparent.color;
            temp.a = transparency;
            transparent.color = temp;

            r.sharedMaterial = transparent;
        }
    }

    public void End()
    {
        foreach (Renderer r in this.GetComponentsInChildren<Renderer>())
        {
            r.sharedMaterial = original;
        }
    }
}
