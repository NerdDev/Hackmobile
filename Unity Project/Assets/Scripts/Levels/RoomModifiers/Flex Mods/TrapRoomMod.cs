using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapRoomMod : RoomModifier
{
    public override bool Unique { get { return true; } }
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
        Counter floorSpace;
        room.Array.DrawSquare(Draw.EqualThen(GridType.Floor, Draw.Count<GridType>(out floorSpace)));
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
        RandomPicker<GridType> picker;
        room.Array.DrawSquare(Draw.IfThen<GridType>(
            Draw.EqualTo(GridType.Floor),
            Draw.PickRandom(room.Array, out picker)));

        List<Value2D<GridType>> treasureList = picker.Pick(Probability.LevelRand, treasureInRoom, 2, true);
        foreach (Value2D<GridType> val in treasureList)
            room.Array[val.y, val.x] = GridType.Chest;

        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            room.ToLog(Logs.LevelGen, "After picking treasure");
        }
        #endregion

        List<Value2D<GridType>> trapList = picker.Pick(Probability.LevelRand, trapsInRoom, 0, true);
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
