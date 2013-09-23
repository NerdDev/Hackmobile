using UnityEngine;
using System.Collections;

public class RectangularRoom : RoomModifier
{

    public override bool Modify(RoomSpec spec)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(DebugManager.Logs.LevelGen, ToString());
        }
        #endregion
        int height = Probability.LevelRand.Next(LevelGenerator.minRectSize, LevelGenerator.maxRectSize);
        int width = Probability.LevelRand.Next(LevelGenerator.minRectSize, LevelGenerator.maxRectSize);
        #region DEBUG
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.w(DebugManager.Logs.LevelGen, "Height: " + height + ", Width: " + width);
        }
        #endregion
        spec.Room.BoxStrokeAndFill(GridType.Wall, GridType.Floor, width, height);
        #region DEBUG
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(DebugManager.Logs.LevelGen);
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
