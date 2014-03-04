using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class HiddenRoomMod : FillRoomMod
{
    public override bool Modify(RoomSpec spec)
    {
        int secretRoomSize = 2;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Hidden Room Mod");
        }
        #endregion

        RandomPicker<GenSpace> picker;
        spec.Grids.DrawPotentialExternalDoors(Draw.PickRandom(out picker));

        Value2D<GenSpace> doorSpace = picker.Pick(spec.Random);
        if (doorSpace == null) return false;
        var floors = new List<Value2D<GenSpace>>();
        spec.Grids.DrawRect(
            (doorSpace.x - secretRoomSize), (doorSpace.x + secretRoomSize),
            (doorSpace.y - secretRoomSize), (doorSpace.y + secretRoomSize),
            new StrokedAction<GenSpace>()
            {
                UnitAction = Draw.IsTypeThen(GridType.NULL, Draw.SetTo(GridType.Floor, spec.Theme).And(Draw.AddTo(floors))),
                StrokeAction = Draw.SetTo(GridType.NULL, new GenSpace(GridType.Wall, spec.Theme))
            });
        bool chest = spec.Random.Percent(.75d);
        if (chest)
        {
            List<Value2D<GenSpace>> chestOptions = new List<Value2D<GenSpace>>();
            spec.Grids.DrawPoints(floors.Cast<Point>(), 
                Draw.Not(Draw.HasAround(false, Draw.IsType<GenSpace>(GridType.Door)))
                .IfThen(Draw.AddTo(chestOptions)));
            spec.Grids[chestOptions.Random(spec.Random)] = new GenSpace(GridType.Chest, spec.Theme);
        }
        else
        {
            spec.Grids[floors.Random(spec.Random)] = new GenSpace(GridType.SmallLoot, spec.Theme);
        }
        spec.Grids.SetTo(doorSpace.x, doorSpace.y, new GenSpace(GridType.Door, spec.Theme));
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            spec.Room.ToLog(Logs.LevelGen, "Final Room After placing doors");
            BigBoss.Debug.printFooter(Logs.LevelGen, "Hidden Room Mod");
        }
        #endregion
        return true;
    }

    public override List<ProbabilityItem<RoomModifier>> GetChainedModifiers()
    {
        return new List<ProbabilityItem<RoomModifier>>(0);
    }
}
