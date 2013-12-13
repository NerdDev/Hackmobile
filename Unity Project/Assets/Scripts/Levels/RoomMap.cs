using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomMap : MultiMap<GridSpace>, IEnumerable  {

    public RoomMap(LayoutObjectLeaf room, GridSpace[,] arr)
    {
        int shiftX = room.GetShift().x;
        int shiftY = room.GetShift().y;
		room.GetArray().GetArr().DrawSquare(Draw.EqualThen(GridType.Floor,
			(arr2, x, y) =>
		{
            this.Put(arr[y + shiftY, x + shiftX], y + shiftY, x + shiftX);	
			return true;
		}));
    }

    public MultiMap<GridSpace> Spawnable()
    {
        return DungeonMaster.Spawnable(this);
    }
}
