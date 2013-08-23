using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class TreasureRoomMod : RoomModifier
{
    public override void Modify(Room room, RandomGen rand)
    {
        Bounding bounds = room.GetBounding();
        int centerX = (bounds.XMin + bounds.XMax) / 2;
        int centerY = (bounds.YMin + bounds.YMax) / 2;
        for (int y = centerY - 2; y <= centerY + 2; y++)
        {
            room.put(GridType.Wall, centerX - 2, y);
            room.put(GridType.Wall, centerX + 2, y);
        }
        for (int x = centerX - 2; x <= centerX + 2; x++)
        {
            room.put(GridType.Wall, x, centerY - 2);
            room.put(GridType.Wall, x, centerY + 2);
        }
        for (int x = centerX - 1; x <= centerX + 1; x++)
        {
            for (int y = centerY - 1; y <= centerY + 1; y++)
            {
                room.put(GridType.Floor, x, y);
            }
        }
        room.put(GridType.Door, centerX, centerY + 2);
        room.put(GridType.Door, centerX, centerY - 2);
        room.put(GridType.Door, centerX + 2, centerY);
        room.put(GridType.Door, centerX - 2, centerY);
        room.put(GridType.Chest, centerX, centerY);
    }

    public override RoomModType GetType()
    {
        return RoomModType.Flexible;
    }

    public override string GetName()
    {
        return "Treasure Room";
    }
}
