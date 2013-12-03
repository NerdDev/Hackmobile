using UnityEngine;
using System.Collections;

public class TrapRoomMod : RoomModifier
{
    protected static ProbabilityList<int> treasureSizeList = new ProbabilityList<int>();
    static TrapRoomMod()
    {
        treasureSizeList.Add(0, .25, false);
        treasureSizeList.Add(1, .5, false);
        treasureSizeList.Add(2, .15, false);
        treasureSizeList.Add(3, .10, false);
        treasureSizeList.ToLog(Logs.Main);
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
        int treasureInRoom = treasureSizeList.Get();
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Treasure In Room: " + treasureInRoom);
        }
        #endregion
        int floorSpace = room.CountGridType(GridType.Floor);
        int trapsInRoom = (floorSpace - treasureInRoom) / 8;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Floor Space: " + floorSpace + " Traps In Room: " + trapsInRoom);
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
