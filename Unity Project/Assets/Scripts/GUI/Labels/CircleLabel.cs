using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class CircleLabel : UILabel
{
    public List<GameObject> sprites;

    public float radii = 80f;

    void Start()
    {
        PlaceInCircle(sprites, transform.localPosition, radii);
    }

    public void PlaceInCircle(List<GameObject> objs, Vector3 center, float radius)
    {
        // create random angle between 0 to 360 degrees
        int numAngles = objs.Count;
        int angleOffset = 360 / numAngles;
        int angle = 0;
        foreach (GameObject obj in objs)
        {
            obj.transform.localPosition = new Vector3(center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad), center.y + radius * Mathf.Cos(angle * Mathf.Deg2Rad), center.z);
            angle += angleOffset;
        }
    }
}