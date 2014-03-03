using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SquareRoom : BaseRoomMod
{
    public override bool Modify(RoomSpec spec)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, ToString());
        }
        #endregion
        int side = spec.Random.Next(LevelGenerator.minRectSize, LevelGenerator.maxRectSize);
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
            UnitAction = Draw.SetTo(new GenSpace(GridType.Floor, spec.Theme)),
            StrokeAction = Draw.SetTo(new GenSpace(GridType.Wall, spec.Theme))
        });
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, ToString());
        }
        #endregion
        return true;
    }

    public override List<ProbabilityItem<RoomModifier>> GetChainedModifiers()
    {
        return new List<ProbabilityItem<RoomModifier>>(0);
    }
}
