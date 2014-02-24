using UnityEngine;
using System.Collections;

public class RectangularRoom : BaseRoomMod
{
    public override RoomModType ModType { get { return RoomModType.Base; } }
    public override string Name { get { return "Rectangular Room"; } }
    public override bool Modify(RoomSpec spec)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, ToString());
        }
        #endregion
        int height = spec.Random.Next(LevelGenerator.minRectSize, LevelGenerator.maxRectSize);
        int width = spec.Random.Next(LevelGenerator.minRectSize, LevelGenerator.maxRectSize);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Height: " + height + ", Width: " + width);
        }
        #endregion
        Point center = spec.Grids.Center;
        int left = center.x - (width / 2);
        int bottom = center.y - (height / 2);
        spec.Grids.DrawSquare(left, left + width, bottom, bottom + height, new StrokedAction<GridSpace>()
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
