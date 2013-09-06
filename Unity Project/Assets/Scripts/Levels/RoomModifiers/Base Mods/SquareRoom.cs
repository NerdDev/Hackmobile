using UnityEngine;
using System.Collections;

public class SquareRoom : RoomModifier {

    public override void Modify(Room room, RandomGen rand)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(DebugManager.Logs.LevelGen, ToString());
        }
        #endregion
        int side = Probability.LevelRand.Next(LevelGenerator.minRectSize, LevelGenerator.maxRectSize);
        #region DEBUG
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.w(DebugManager.Logs.LevelGen, "Side: " + side);
        }
        #endregion
        room.BoxStrokeAndFill(GridType.Wall, GridType.Floor, side, side);
        #region DEBUG
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(DebugManager.Logs.LevelGen);
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
