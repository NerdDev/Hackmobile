using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomMap : MultiMap<GridSpace>, IEnumerable  {

    public RoomMap(LayoutObjectLeaf room, Container2D<GridSpace> arr)
    {
        int shiftX = room.GetShift().x;
        int shiftY = room.GetShift().y;
		room.GetArray().DrawSquare(Draw.EqualThen(GridType.Floor,
			(arr2, x, y) =>
		{
            this.Put(arr[x + shiftX, y + shiftY], x + shiftX, y + shiftY);	
			return true;
		}));
    }

    public MultiMap<GridSpace> Spawnable()
    {
        return DungeonMaster.Spawnable(this);
    }
}
