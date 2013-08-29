using UnityEngine;
using System.Collections;

public class GiantPillarMod : RoomModifier
{

    public override void Modify(Room room, RandomGen rand)
    {
        Bounding bounds = room.GetBounding();
        int x = rand.Next(bounds.XMin + 3, bounds.XMax - 3);
        int y = rand.Next(bounds.YMin + 3, bounds.YMax - 3);
        int i = rand.Next(2, 5);

        for (int xx = x; xx < x + i; xx++)
        {
            for (int yy = y; yy < y + i; yy++)
            {
                if (room.get(xx, yy) == GridType.Floor || room.get(xx, yy) == GridType.Trap) room.put(GridType.Wall, xx, yy);
            }
        }
    }

    public override RoomModType GetType()
    {
        return RoomModType.Flexible;
    }

    public override string GetName()
    {
        return "Giant Pillar";
    }
}
