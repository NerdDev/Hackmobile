using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CircleRoom : BaseRoomMod
{
    const int minRadiusSize = 6;
    const int maxRadiusSize = 10;
    public override bool Modify(RoomSpec spec)
    {
        int radius = spec.Random.Next(minRadiusSize, maxRadiusSize);
        int center = spec.Room.Grids.Width / 2;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Radius: " + radius);
        }
        #endregion
        spec.Grids.DrawCircle(center, center, radius, new StrokedAction<GenSpace>()
            {
                UnitAction = Draw.SetTo(new GenSpace(GridType.Floor, spec.Theme)),
                StrokeAction = Draw.SetTo(new GenSpace(GridType.Wall, spec.Theme))
            });
        return true;
    }

    public override List<ProbabilityItem<RoomModifier>> GetChainedModifiers()
    {
        return new List<ProbabilityItem<RoomModifier>>(0);
    }
}
