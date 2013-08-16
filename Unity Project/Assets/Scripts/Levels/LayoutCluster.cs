using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayoutCluster : LayoutObjectContainer {

    public override void AddObject(LayoutObject r)
    {
        Point shiftDir = LevelGenerator.GenerateShiftMagnitude(1);
        LayoutObject intersect;
        while ((intersect = r.intersectObj(Objects, 0)) != null)
        {
            r.ShiftOutside(intersect, shiftDir);
        }
        base.AddObject(r);
    }

    public override string GetTypeString()
    {
        return "Room Cluster";
    }
}
