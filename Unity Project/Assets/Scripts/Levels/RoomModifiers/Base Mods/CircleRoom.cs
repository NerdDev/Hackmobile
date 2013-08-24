using UnityEngine;
using System.Collections;

public class CircleRoom : RoomModifier {

    public override void Modify(Room room, RandomGen rand)
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, ToString());
        }
        #endregion
        int radius = Probability.LevelRand.Next(LevelGenerator.minRadiusSize, LevelGenerator.maxRadiusSize);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.w(DebugManager.Logs.LevelGen, "Radius: " + radius);
        }
        #endregion
        room.CircularStrokeAndFill(GridType.Wall, GridType.Floor, radius);
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
        return "Circular Room";
    }
}
