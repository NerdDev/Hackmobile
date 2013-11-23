using UnityEngine;
using System.Collections;

public class HiddenRoomMod : RoomModifier {

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
            GridMap potentialDoors = room.GetPotentialExternalDoors();
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                potentialDoors.ToLog(Logs.LevelGen, "After Removing Invalid Locations");
            }
            #endregion
            Value2D<GridType> doorSpace = potentialDoors.RandomValue(spec.Random);
                if (doorSpace != null)
                {
                    room.Array.DrawSquare(
                        (doorSpace.x-secretRoomSize),(doorSpace.x+secretRoomSize),
                        (doorSpace.y-secretRoomSize),(doorSpace.y+secretRoomSize),
                        new StrokedAction<GridType>()
                        {
                            UnitAction = SetToIfNull(GridType.Floor),
                            StrokeAction = SetToIfNull(GridType.Wall)
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
