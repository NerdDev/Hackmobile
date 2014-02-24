using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapRoomMod : FillRoomMod
{
    public override RoomModType ModType { get { return RoomModType.Flexible; } }
    public override string Name { get { return "Trap Room"; } }
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
        LayoutObject room = spec.Room;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, ToString());
        }
        #endregion
        Counter floorSpace;
        RandomPicker<GridSpace> picker;
        spec.Grids.DrawAll(Draw.IsTypeThen(GridType.Floor, Draw.Count<GridSpace>(out floorSpace).And(Draw.PickRandom(out picker))));
        if (floorSpace < 15)
        {
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "Room was too small: " + floorSpace);
            }
            #endregion
            return false;
        }
        int treasureInRoom = treasureSizeList.Get(spec.Random);
        int trapsInRoom = (floorSpace - treasureInRoom) / 8;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Treasure In Room: " + treasureInRoom);
            BigBoss.Debug.w(Logs.LevelGen, "Floor Space: " + floorSpace + " Traps In Room: " + trapsInRoom);
        }
        #endregion

        List<Value2D<GridSpace>> treasureList = picker.Pick(spec.Random, treasureInRoom, 2, true);
        foreach (Value2D<GridSpace> val in treasureList)
            spec.Grids.SetTo(val.x, val.y, GridType.Chest);

        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            room.ToLog(Logs.LevelGen, "After picking treasure");
        }
        #endregion

        List<Value2D<GridSpace>> trapList = picker.Pick(spec.Random, trapsInRoom, 0, true);
        foreach (Value2D<GridSpace> val in trapList)
            spec.Grids.SetTo(val.x, val.y, GridType.Trap);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, ToString());
        }
        #endregion
        return true;
    }
}
