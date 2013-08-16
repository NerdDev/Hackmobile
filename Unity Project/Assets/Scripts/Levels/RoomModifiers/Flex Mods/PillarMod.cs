using UnityEngine;
using System.Collections;

public class PillarMod : RoomModifier {

    public override void Modify(Room room, System.Random rand)
    {
        Bounding bounds = room.GetBounding();
        for (int x = bounds.XMin; x < bounds.XMax; x = x + 3)
        {
            for (int y = bounds.YMin; y < bounds.YMax; y = y + 3)
            {
                if (room.get(x, y) == GridType.Floor) room.put(GridType.Wall, x, y);
            }
        }
    }

    public override RoomModType GetType()
    {
        return RoomModType.Flexible;
    }

    public override string GetName()
    {
        return "Pillars";
    }
}
