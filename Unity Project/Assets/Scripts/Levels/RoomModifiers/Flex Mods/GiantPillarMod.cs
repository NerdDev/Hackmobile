using UnityEngine;
using System.Collections;

public class GiantPillarMod : RoomModifier
{

    public override bool Modify(RoomSpec spec)
    {
        Bounding bounds = spec.Room.GetBounding();
        int x = spec.Random.Next(bounds.XMin + 3, bounds.XMax - 3);
        int y = spec.Random.Next(bounds.YMin + 3, bounds.YMax - 3);
        int i = spec.Random.Next(2, 5);

        for (int xx = x; xx < x + i; xx++)
        {
            for (int yy = y; yy < y + i; yy++)
            {
                if (spec.Room.get(xx, yy) == GridType.Floor || spec.Room.get(xx, yy) == GridType.Trap) spec.Room.put(GridType.Wall, xx, yy);
            }
        }
        return true;
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
