using UnityEngine;
using System.Collections;

public class RoomMap : MultiMap<GridSpace>, IEnumerable  {

    public RoomMap(Room room, GridSpace[,] arr)
    {
        foreach (Point<GridType> floor in room.GetFloors())
        {
            this.Put(arr[floor.y, floor.x], floor.x, floor.y);
        }
    }

    public MultiMap<GridSpace> Spawnable()
    {
        return DungeonMaster.Spawnable(this);
    }
}
