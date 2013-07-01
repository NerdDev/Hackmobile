using UnityEngine;
using System.Collections;

public class PillarMod : RoomModifier {

    public override void Modify(Room room, System.Random rand)
    {
        Bounding bounds = room.GetBounding();
        for (int x = bounds.xMin; x < bounds.xMax; x = x + 3)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y = y + 3)
            {
                room.put(GridType.Wall, x, y);
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
