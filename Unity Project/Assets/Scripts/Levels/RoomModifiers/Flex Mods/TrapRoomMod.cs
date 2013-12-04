using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapRoomMod : RoomModifier
{
    protected static ProbabilityList<int> treasureSizeList = new ProbabilityList<int>();
    static TrapRoomMod()
    {
        treasureSizeList.Add(0, .25, false);
        treasureSizeList.Add(1, .5, false);
        treasureSizeList.Add(2, .15, false);
        treasureSizeList.Add(3, .10, false);
    }

    public override bool Modify(RoomSpec spec)
    {
        RandomGen rand = spec.Random;
        Room room = spec.Room;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, ToString());
        }
        #endregion
        int floorSpace = room.CountGridType(GridType.Floor);
        #region DEBUG
        if (floorSpace < 15)
        {
            BigBoss.Debug.w(Logs.LevelGen, "Room was too small: " + floorSpace);
            return false;
        }
        #endregion
        int treasureInRoom = treasureSizeList.Get();
        int trapsInRoom = (floorSpace - treasureInRoom) / 8;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Treasure In Room: " + treasureInRoom);
            BigBoss.Debug.w(Logs.LevelGen, "Floor Space: " + floorSpace + " Traps In Room: " + trapsInRoom);
        }
        #endregion
        GridMap grid = room.GetFloors();
        List<Value2D<GridType>> treasureList = grid.GetRandomRemove(treasureInRoom);
        foreach (Value2D<GridType> val in treasureList)
            room.Array[val.y, val.x] = GridType.Chest;

        List<Value2D<GridType>> trapList = grid.GetRandomRemove(treasureInRoom);
        foreach (Value2D<GridType> val in trapList)
            room.Array[val.y, val.x] = GridType.Trap;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
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
        return "Trap Room";
    }
}
