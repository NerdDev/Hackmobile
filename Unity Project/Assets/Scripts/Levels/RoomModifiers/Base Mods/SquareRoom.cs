using UnityEngine;
using System.Collections;

public class SquareRoom : BaseRoomMod
{
    public override RoomModType ModType { get { return RoomModType.Base; } }
    public override string Name { get { return "Square Room"; } }
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
        spec.Grids.DrawSquare(left, left + side, bottom, bottom + side, new StrokedAction<GridSpace>()
        {
            UnitAction = Draw.SetTo(GridType.Floor),
            StrokeAction = Draw.SetTo(GridType.Wall)
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
