using UnityEngine;
using System.Collections;

public class CircleRoom : RoomModifier {

    public override void Modify(Room room, RandomGen rand)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(DebugManager.Logs.LevelGen, ToString());
        }
        #endregion
        int radius = Probability.LevelRand.Next(LevelGenerator.minRadiusSize, LevelGenerator.maxRadiusSize);
        #region DEBUG
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.w(DebugManager.Logs.LevelGen, "Radius: " + radius);
        }
        #endregion
        room.CircularStrokeAndFill(GridType.Wall, GridType.Floor, radius);
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
        return "Circular Room";
    }
}
