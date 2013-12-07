using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HiddenRoomMod : RoomModifier
{
    public override bool Modify(RoomSpec spec)
    {
        Room room = spec.Room;
        int secretRoomSize = 2;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Hidden Room Mod");
        }
        #endregion
        GridMap potentialDoors = new GridMap();
        room.Array.DrawPotentialExternalDoors(Draw.AddTo(potentialDoors));
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            potentialDoors.ToLog(Logs.LevelGen, "After Removing Invalid Locations");
        }
        #endregion
        Value2D<GridType> doorSpace = potentialDoors.RandomValue(Probability.LevelRand);
        if (doorSpace != null)
        {
            room.Array.DrawSquare(
                (doorSpace.x - secretRoomSize), (doorSpace.x + secretRoomSize),
                (doorSpace.y - secretRoomSize), (doorSpace.y + secretRoomSize),
                new StrokedAction<GridType>()
                {
                    UnitAction = Draw.SetTo(GridType.NULL, GridType.Floor),
                    StrokeAction = Draw.SetTo(GridType.NULL, GridType.Wall)
                });
            room.put(GridType.Door, doorSpace.x, doorSpace.y);
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            room.ToLog(Logs.LevelGen, "Final Room After placing doors");
            BigBoss.Debug.printFooter(Logs.LevelGen);
        }
        #endregion
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            //layout.ToLog(Logs.LevelGen);
            BigBoss.Debug.printFooter(Logs.LevelGen);
        }
        #endregion
        return true;
    }

    public override RoomModType GetType()
    {
        return RoomModType.Flexible;
    }

    public override string GetName()
    {
        return "Hidden Room";
    }
}
