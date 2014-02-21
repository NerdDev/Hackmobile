using UnityEngine;
using System.Collections;

public class CircleRoom : BaseRoomMod
{
    public override RoomModType ModType { get { return RoomModType.Base; } }
    public override string Name { get { return "Circular Room"; } }

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
        spec.Grids.DrawCircle(center, center, radius, new StrokedAction<GridType>()
            {
                UnitAction = Draw.SetTo(GridType.Floor),
                StrokeAction = Draw.SetTo(GridType.Wall)
            });
        return true;
    }
}
