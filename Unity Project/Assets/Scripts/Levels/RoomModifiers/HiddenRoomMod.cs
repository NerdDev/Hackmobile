using UnityEngine;
using System.Collections;

public class HiddenRoomMod : RoomModifier {

  public override void Modify(Room room, System.Random rand)
    {
        int doorsMin = 0;
        int doorsMax = 0;
        int minDoorSpacing = 0;
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Hidden Room Mod");
        }
        #endregion
            GridMap potentialDoors = room.getWalls();
            GridMap corneredAreas = room.getCorneredBy(GridType.Wall, GridType.Wall);
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                potentialDoors.toLog(DebugManager.Logs.LevelGen, "Original Room");
                corneredAreas.toLog(DebugManager.Logs.LevelGen, "Cornered Areas");
            }
            #endregion
            potentialDoors.RemoveAll(corneredAreas);
            int numDoors = rand.Next(doorsMin, doorsMax);
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                potentialDoors.toLog(DebugManager.Logs.LevelGen, "After Removing Invalid Locations");
                DebugManager.w(DebugManager.Logs.LevelGen, "Number of doors to generate: " + numDoors);
            }
            #endregion
            for (int i = 0; i < numDoors; i++)
            {
                Value2D<GridType> doorSpace = potentialDoors.RandomValue(rand);
                if (doorSpace != null)
                {
                    room.put(GridType.Door, doorSpace.x, doorSpace.y);
                    potentialDoors.Remove(doorSpace.x, doorSpace.y, minDoorSpacing - 1);
                    #region DEBUG
                    if (DebugManager.logging(DebugManager.Logs.LevelGen))
                    {
                        room.toLog(DebugManager.Logs.LevelGen, "Generated door at: " + doorSpace);
                        potentialDoors.toLog(DebugManager.Logs.LevelGen, "Remaining options");
                    }
                    #endregion
                }
                #region DEBUG
                else if (DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    DebugManager.w(DebugManager.Logs.LevelGen, "No door options remain.");
                }
                #endregion
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

    public override int GetProbabilityDivider()
    {
        return 1;
    }
}
