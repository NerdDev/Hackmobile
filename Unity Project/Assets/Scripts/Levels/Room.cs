using UnityEngine;
using System.Collections;
using System;

public class Room : MapObject {

    int relativeX = 0;
    int relativeY = 0;
    MultiMap<GridBox> grids = new MultiMap<GridBox>();

    public override GridBox get(int x, int y)
    {
        throw new NotImplementedException();
    }

    public override MultiMap<GridBox> getFlat()
    {
        throw new NotImplementedException();
    }
}
