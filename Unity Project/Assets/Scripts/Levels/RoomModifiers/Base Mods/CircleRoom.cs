using UnityEngine;
using System.Collections;

public class CircleRoom : RoomModifier {

    public override bool Modify(RoomSpec spec)
    {
        int radius = spec.Random.Next(LevelGenerator.minRadiusSize, LevelGenerator.maxRadiusSize);
        int center = spec.Room.Grids.Width / 2;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Radius: " + radius);
        }
        #endregion
        spec.Grids.DrawCircle(center, center, radius, GridType.Floor, GridType.Wall);
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
