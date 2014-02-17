using System;
using UnityEngine;
using System.Collections.Generic;

public class ItemChest : MonoBehaviour
{
    public GridSpace Location;

    public void init()
    {
        Vector3 blockPos = Location.Block.transform.position;
        this.transform.localPosition = new Vector3(blockPos.x + .3f, blockPos.y + .5f, blockPos.z + .3f);
        this.transform.Rotate(Vector3.up, 45f);
    }

    void OnMouseDown()
    {
        if (CheckDistance())
        {
            BigBoss.Gooey.OpenGroundGUI(this);
        }
    }

    internal bool CheckDistance()
    {
        if (Location.Equals(BigBoss.Player.GridSpace))
        {
            return true;
        }
        Value2D<GridSpace> space;
        GridSpace playerSpace = BigBoss.Player.GridSpace;
        return BigBoss.Levels.Level.Array.GetPointAround(playerSpace.X, playerSpace.Y, true, (arr, x, y) =>
        {
            return Location.X == x && Location.Y == y;
        }, out space);
    }
}
