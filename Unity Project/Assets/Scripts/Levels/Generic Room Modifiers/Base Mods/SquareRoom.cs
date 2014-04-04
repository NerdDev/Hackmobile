using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SquareRoom : BaseRoomMod
{
    public int MinSize = 10;
    public int MaxSize = 20;

    public SquareRoom()
    {
    }

    protected override bool ModifyInternal(RoomSpec spec, double scale)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, ToString());
        }
        #endregion
        int min = (int)(MinSize * scale);
        int max = (int)(MaxSize * scale);
        int side = spec.Random.Next(min, max);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Side: " + side);
        }
        #endregion
        Point center = spec.Grids.Center;
        int left = center.x - (side / 2);
        int bottom = center.y - (side / 2);
        spec.Grids.DrawRect(left, left + side, bottom, bottom + side, new StrokedAction<GenSpace>()
        {
            UnitAction = Draw.SetTo(GridType.Floor, spec.Theme),
            StrokeAction = Draw.SetTo(GridType.Wall, spec.Theme)
        });
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, ToString());
        }
        #endregion
        return true;
    }
}
