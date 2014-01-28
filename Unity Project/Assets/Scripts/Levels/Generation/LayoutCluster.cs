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
        r.ShiftOutside(Objects, shiftDir);
        base.AddObject(r);
    }

}
