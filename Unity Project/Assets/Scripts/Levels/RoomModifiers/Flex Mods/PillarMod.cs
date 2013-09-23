using UnityEngine;
using System.Collections;

public class PillarMod : RoomModifier {

    public override bool Modify(RoomSpec spec)
    {
        Bounding bounds = spec.Room.GetBounding();
        for (int x = bounds.XMin; x < bounds.XMax; x = x + 3)
        {
            for (int y = bounds.YMin; y < bounds.YMax; y = y + 3)
            {
                if (spec.Room.get(x, y) == GridType.Floor || spec.Room.get(x, y) == GridType.Trap) spec.Room.put(GridType.Wall, x, y);
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
        return "Pillars";
    }

    public override bool IsUnique()
    {
        return true;
    }
}
