using UnityEngine;
using System.Collections;

public class HiddenRoomMod : RoomModifier {

  public override void Modify(Room room, System.Random rand)
    {
        int secretRoomSize = 2;
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Hidden Room Mod");
        }
        #endregion
            GridMap potentialDoors = room.GetPotentialDoors();
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                potentialDoors.toLog(DebugManager.Logs.LevelGen, "After Removing Invalid Locations");
            }
            #endregion
            Value2D<GridType> doorSpace = potentialDoors.RandomValue(rand);
                if (doorSpace != null)
                { 
                    room.BoxStrokeAndFill(GridType.Wall,GridType.Floor,
                        (doorSpace.x-secretRoomSize),(doorSpace.x+secretRoomSize),
                        (doorSpace.y-secretRoomSize),(doorSpace.y+secretRoomSize));
                    room.put(GridType.Door, doorSpace.x, doorSpace.y);
                }
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                room.toLog(DebugManager.Logs.LevelGen, "Final Room After placing doors");
                DebugManager.printFooter(DebugManager.Logs.LevelGen);
            }
            #endregion
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            //layout.toLog(DebugManager.Logs.LevelGen);
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
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
