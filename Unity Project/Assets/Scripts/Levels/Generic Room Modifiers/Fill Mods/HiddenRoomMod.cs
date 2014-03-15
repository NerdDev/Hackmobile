using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class HiddenRoomMod : FillRoomMod
{
    protected override bool ModifyInternal(RoomSpec spec)
    {
        int secretRoomSize = 2;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Hidden Room Mod");
            spec.Grids.ToLog(Logs.LevelGen, "Final Room After placing doors");
        }
        #endregion

        RandomPicker<GenSpace> picker;
        spec.Grids.DrawPotentialExternalDoors(Draw.PickRandom(out picker));

        Value2D<GenSpace> doorSpace;
        if (!picker.Pick(spec.Random, out doorSpace)) return false;
        var floors = new List<Value2D<GenSpace>>();
        spec.Grids.DrawRect(
            (doorSpace.x - secretRoomSize), (doorSpace.x + secretRoomSize),
            (doorSpace.y - secretRoomSize), (doorSpace.y + secretRoomSize),
            new StrokedAction<GenSpace>()
            {
                UnitAction = Draw.IsTypeThen(GridType.NULL, Draw.SetTo(GridType.Floor, spec.Theme).And(Draw.AddTo(floors))),
                StrokeAction = Draw.IsType<GenSpace>(GridType.NULL).IfThen(Draw.SetTo(GridType.Wall, spec.Theme))
            });
        bool chest = spec.Random.Percent(.75d);
        if (chest)
        {
            List<Value2D<GenSpace>> chestOptions = new List<Value2D<GenSpace>>();
            spec.Grids.DrawPoints(floors.Cast<Point>(), 
                Draw.Not(Draw.HasAround(false, Draw.IsType<GenSpace>(GridType.Door)))
                .IfThen(Draw.AddTo(chestOptions)));
            spec.Grids.SetTo(chestOptions.Random(spec.Random), GridType.Chest, spec.Theme);
        }
        else
        {
            spec.Grids.SetTo(floors.Random(spec.Random), GridType.SmallLoot, spec.Theme);
        }
        spec.Grids.SetTo(doorSpace, GridType.Door, spec.Theme);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            spec.Grids.ToLog(Logs.LevelGen, "Final Room After placing doors");
            BigBoss.Debug.printFooter(Logs.LevelGen, "Hidden Room Mod");
        }
        #endregion
        return true;
    }
}
