using UnityEngine;
using System.Collections;

public class RoomMap : MultiMap<GridSpace>, IEnumerable  {

    public RoomMap(Room room, GridSpace[,] arr)
    {
        foreach (Value2D<GridType> floor in room.GetFloors())
        {
            this.Put(arr[floor.y, floor.x], floor.x, floor.y);
        }
    }

    public MultiMap<GridSpace> Spawnable()
    {
        return DungeonMaster.Spawnable(this);
    }
}
