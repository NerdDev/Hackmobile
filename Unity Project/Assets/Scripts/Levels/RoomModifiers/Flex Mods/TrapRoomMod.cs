using UnityEngine;
using System.Collections;

public class TrapRoomMod : RoomModifier
{

    public override void Modify(Room room, RandomGen rand)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(DebugManager.Logs.LevelGen, ToString());
        }
        #endregion
        int treasurePercent = rand.Next(0, 100);
        int treasureInRoom;
        if (treasurePercent <= 20) treasureInRoom = 0;
        else if (treasurePercent <= 75) treasureInRoom = 1;
        else if (treasurePercent <= 90) treasureInRoom = 2;
        else treasureInRoom = 3;
        #region DEBUG
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.w(DebugManager.Logs.LevelGen, "Treasure Percent: " + treasurePercent + " Treasure In Room: " + treasureInRoom);
        }
        #endregion
        int floorSpace = room.CountGridType(GridType.Floor);
        int trapsInRoom = (floorSpace - treasureInRoom) / 8;
        #region DEBUG
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.w(DebugManager.Logs.LevelGen, "Floor Space: " + floorSpace + " Traps In Room: " + trapsInRoom);
        }
        #endregion
        GridMap grid = room.GetFloors();
        Stack stk = new Stack();
        while (treasureInRoom > 0)
        {
            int x = rand.Next(floorSpace-1);
            if (x < 0) break;
            if (!stk.Contains(x))
            {
                room.put(GridType.Chest, grid.GetNth(x).x, grid.GetNth(x).y);
                stk.Push(x);
                treasureInRoom--;
            }
        }

        while (trapsInRoom > 0)
        {
            int x = rand.Next(floorSpace - 1);
            if (x < 0) break;
            if (!stk.Contains(x))
            {
                room.put(GridType.Trap, grid.GetNth(x).x, grid.GetNth(x).y);
                stk.Push(x);
                trapsInRoom--;
            }
        }
        #region DEBUG
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
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
        return "Trap Room";
    }
}
