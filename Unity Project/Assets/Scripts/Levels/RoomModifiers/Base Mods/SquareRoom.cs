using UnityEngine;
using System.Collections;

public class SquareRoom : RoomModifier {

    public override void Modify(Room room, System.Random rand)
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, ToString());
        }
        #endregion
        int height = LevelGenerator.rand.Next(LevelGenerator.minRectSize, LevelGenerator.maxRectSize);
        int width = LevelGenerator.rand.Next(LevelGenerator.minRectSize, LevelGenerator.maxRectSize);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.w(DebugManager.Logs.LevelGen, "Height: " + height + ", Width: " + width);
        }
        #endregion
        room.BoxStrokeAndFill(GridType.Wall, GridType.Floor, width, height);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
    }

    public override RoomModType GetType()
    {
        return RoomModType.Base;
    }

    public override string GetName()
    {
        return "Square Room";
    }
}
