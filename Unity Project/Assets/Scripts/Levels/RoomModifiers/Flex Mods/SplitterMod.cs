using UnityEngine;
using System.Collections;

public class SplitterMod : RoomModifier
{

    public override void Modify(Room room, RandomGen rand)
    {
        Bounding bounds = room.GetBounding();
        int x = rand.Next(bounds.XMin + 3, bounds.XMax - 3);
        int y = rand.Next(bounds.YMin + 3, bounds.YMax - 3);

        for (int i = bounds.XMin; i < bounds.XMax; i++)
        {
            if (room.get(i, y) == GridType.Floor || room.get(i, y) == GridType.Trap) room.put(GridType.Wall, i, y);
        }
        for (int i = bounds.YMin; i < bounds.YMax; i++)
        {
            if (room.get(x, i) == GridType.Floor || room.get(x, i) == GridType.Trap) room.put(GridType.Wall, x, i);
        }

        int ctr = 0;

        while (ctr < x - bounds.XMin - 1)
        {
            int r = rand.Next(bounds.XMin, x - 1);
            if (room.get(r, y - 1) != GridType.Wall && room.get(r, y + 1) != GridType.Wall)
            {
                room.put(GridType.Door, r, y);
                break;
            }
            else ctr++;
        }
        ctr = 0;
        while (ctr < bounds.XMax - x + 1)
        {
            int r = rand.Next(x + 1, bounds.XMax);
            if (room.get(r, y - 1) != GridType.Wall && room.get(r, y + 1) != GridType.Wall)
            {
                room.put(GridType.Door, r, y);
                break;
            }
            else ctr++;
        }
        ctr = 0;
        while (ctr < y - bounds.YMin - 1)
        {
            int r = rand.Next(bounds.YMin, y - 1);
            if (room.get(x - 1, r) != GridType.Wall && room.get(x + 1, r) != GridType.Wall)
            {
                room.put(GridType.Door, x, r);
                break;
            }
            else ctr++;
        }
        ctr = 0;
        while (ctr < bounds.YMax - y + 1)
        {
            int r = rand.Next(y + 1, bounds.YMax);
            if (room.get(x - 1, r) != GridType.Wall && room.get(x + 1, r) != GridType.Wall)
            {
                room.put(GridType.Door, x, r);
                break;
            }
            else ctr++;
        }
        ctr = 0;
    }

    public override RoomModType GetType()
    {
        return RoomModType.Flexible;
    }

    public override string GetName()
    {
        return "Splitter";
    }
}
