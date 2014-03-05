using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CircleRoom : BaseRoomMod
{
    public int MinRadius = 6;
    public int MaxRadius = 10;

    public CircleRoom ()
    {
    }

    protected override bool ModifyInternal(RoomSpec spec, double scale)
    {
        int radius = spec.Random.Next((int)(MinRadius * scale), (int)(MaxRadius * scale));
        int center = spec.Grids.Width / 2;
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
}
