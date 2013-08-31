using UnityEngine;
using System.Collections;

public class HiddenRoomMod : RoomModifier {

    public override void Modify(Room room, RandomGen rand)
    {
        int secretRoomSize = 2;
        #region DEBUG
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(DebugManager.Logs.LevelGen, "Hidden Room Mod");
        }
        #endregion
            GridMap potentialDoors = room.GetPotentialExternalDoors();
            #region DEBUG
            if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
            {
                potentialDoors.ToLog(DebugManager.Logs.LevelGen, "After Removing Invalid Locations");
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
            if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
            {
                room.ToLog(DebugManager.Logs.LevelGen, "Final Room After placing doors");
                BigBoss.Debug.printFooter(DebugManager.Logs.LevelGen);
            }
            #endregion
        #region DEBUG
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            //layout.ToLog(DebugManager.Logs.LevelGen);
            BigBoss.Debug.printFooter(DebugManager.Logs.LevelGen);
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
