using UnityEngine;
using System.Collections;

public class CircleRoom : RoomModifier {

    public override bool Modify(RoomSpec spec)
    {
        int radius = Probability.LevelRand.Next(LevelGenerator.minRadiusSize, LevelGenerator.maxRadiusSize);
        int center = spec.Room.Width / 2;
        spec.Room.Array.DrawCircle(center, center, radius, GridType.Floor, GridType.Wall);
        return true;
    }

    public override RoomModType GetType()
    {
        return RoomModType.Base;
    }

    public override string GetName()
    {
        return "Circular Room";
    }
}
