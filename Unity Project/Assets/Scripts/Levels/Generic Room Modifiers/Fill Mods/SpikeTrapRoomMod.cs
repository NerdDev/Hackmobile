using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class SpikeTrapRoomMod : FillRoomMod
{
    public override bool Unique { get { return true; } }
    protected static ProbabilityList<int> treasureSizeList = new ProbabilityList<int>();
    static SpikeTrapRoomMod()
    {
        treasureSizeList.Add(0, .25, false);
        treasureSizeList.Add(1, .5, false);
        treasureSizeList.Add(2, .15, false);
        treasureSizeList.Add(3, .10, false);
    }

    protected override bool ModifyInternal(RoomSpec spec)
    {
        ISpikeTrapTheme spikeTheme = EnsureThemeImplements<ISpikeTrapTheme>(spec);
        Counter floorSpace;
        RandomPicker<GenSpace> picker;
        spec.Grids.DrawAll(Draw.EmptyAndFloor<GenSpace>().IfThen(Draw.Count<GenSpace>(out floorSpace).And(Draw.PickRandom(out picker))));
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

        List<Value2D<GenSpace>> treasureList = picker.Pick(spec.Random, treasureInRoom, 2, true);
        foreach (Value2D<GenSpace> val in treasureList)
        {
            spec.Grids.SetTo(val, GridType.Chest, spec.Theme);
        }

        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            spec.Grids.ToLog(Logs.LevelGen, "After picking treasure");
        }
        #endregion

        List<Value2D<GenSpace>> trapList = picker.Pick(spec.Random, trapsInRoom, 0, true);
        PlaceDoodads(spec, spikeTheme.GetSpikeTraps(), trapList.Cast<Point>());
        return true;
    }
}
