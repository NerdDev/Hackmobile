using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayoutCluster : LayoutObjectContainer {

    System.Random rand;
    public LayoutCluster(System.Random rand)
    {
        this.rand = rand;
    }

    public override void AddObject(LayoutObject r)
    {
        Point shiftDir = LevelGenerator.GenerateShiftMagnitude(5, rand);
        LayoutObject intersect;
        while ((intersect = r.IntersectSmart(Objects)) != null)
        {
            r.ShiftOutside(intersect, shiftDir, false, false);
        }
        base.AddObject(r);
    }

    public override string GetTypeString()
    {
        return "Room Cluster";
    }
}
