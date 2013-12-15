using UnityEngine;
using System.Collections;

public class RectangularRoom : RoomModifier
{

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
        spec.Array.DrawSquare(width, height, new StrokedAction<GridType>()
            {
                UnitAction = Draw.SetTo(GridType.Floor),
                StrokeAction = Draw.SetTo(GridType.Wall)
            });
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen);
        }
        #endregion
        return true;
    }

    public override RoomModType GetType()
    {
        return RoomModType.Base;
    }

    public override string GetName()
    {
        return "Rectangular Room";
    }
}
