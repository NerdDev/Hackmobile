using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomCluster : LayoutObjectContainer {

    List<Room> cluster = new List<Room>();

    public void AddRoom(Room r)
    {
        Point shiftDir = LevelGenerator.GenerateShiftMagnitude(1);
        LayoutObject intersect;
        while ((intersect = r.intersectObj(cluster, 0)) != null)
        {
            r.ShiftOutside(intersect, shiftDir);
        }
    }

    public override string GetTypeString()
    {
        return "Room Cluster";
    }
}
