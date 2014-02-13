using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HorizSplitterMod : RoomModifier
{

    public override bool Modify(RoomSpec spec)
    {
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Splitter Mod");
        }
        #endregion
        Bounding bounds = spec.Room.GetBounding(false);
        List<int> options = new List<int>();
        bool horizontal = spec.Random.NextBool();
        int from = bounds.GetMin(horizontal);
        int to = bounds.GetMax(horizontal);
        int fromAlt = bounds.GetMin(!horizontal);
        int toAlt = bounds.GetMax(!horizontal);

        // Iterate and find all viable options
        for (int i = fromAlt; i <= toAlt; i++)
        {
            Counter floorCount;
            Counter side1;
            Counter side2;
            if (spec.Grids.DrawLine(from, to, i, horizontal,
                // If no doors around
                Draw.Not(Draw.Around(false, Draw.EqualTo(GridType.Door)))
                // Not blocking walking
                .And(Draw.Not(Draw.Blocking<GridType>(Draw.Walkable())))
                // Count floors on line as well as sides
                .And(Draw.EqualTo(GridType.Floor).IfThen(Draw.Count<GridType>(out floorCount)))
                .And(Draw.Loc(horizontal ? GridLocation.UP : GridLocation.LEFT,
                    Draw.EqualThen(GridType.Floor, Draw.Count<GridType>(out side1))))
                .And(Draw.Loc(horizontal ? GridLocation.DOWN : GridLocation.RIGHT,
                    Draw.EqualThen(GridType.Floor, Draw.Count<GridType>(out side2)))))
                // Has a floor in each
                && floorCount > 0 && side1 > 0 && side2 > 0
                )
                options.Add(i);
        }
        if (options.Count == 0) return false;
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
        {
            foreach (int i in options)
            {
                Container2D<GridType> copy = new Array2D<GridType>(spec.Room.Grids);
                copy.DrawLine(from, to, i, horizontal, Draw.SetToIfNotEqual(GridType.NULL, GridType.INTERNAL_RESERVED_BLOCKED));
                copy.ToLog(Logs.LevelGen);
            }
        }
        #endregion

        // Draw selected splitter
        int picked = options.Random(spec.Random);
        spec.Grids.DrawLine(from, to, picked, horizontal, Draw.SetToIfNotEqual(GridType.NULL, GridType.Wall));
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Picked splitter:");
            spec.Room.ToLog(Logs.LevelGen);
        }
        #endregion

        // Draw at least one door
        RandomPicker<GridType> picker;
        spec.Grids.DrawLine(from, to, picked, horizontal, 
            Draw.CanDrawDoor().IfThen(Draw.PickRandom(out picker)));
        
        int numDoors = spec.Random.Next(1, 4);
        List<Value2D<GridType>> doors = picker.Pick(spec.Random, numDoors, 1, false);
        foreach (Value2D<GridType> door in doors)
            spec.Grids[door.x, door.y] = GridType.Door;

        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Splitter Mod");
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
        return "Splitter";
    }
}
