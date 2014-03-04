using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SplitterMod : HeavyRoomMod
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
                Draw.Not(Draw.Around(false, Draw.IsType<GenSpace>(GridType.Door)))
                // Not blocking walking
                .And(Draw.Not(Draw.Blocking<GenSpace>(Draw.Walkable<GenSpace>())))
                // Count floors on line as well as sides
                .And(Draw.IsType<GenSpace>(GridType.Floor).IfThen(Draw.Count<GenSpace>(out floorCount)))
                .And(Draw.Loc(horizontal ? GridLocation.TOP : GridLocation.LEFT,
                    Draw.IsTypeThen(GridType.Floor, Draw.Count<GenSpace>(out side1))))
                .And(Draw.Loc(horizontal ? GridLocation.BOTTOM : GridLocation.RIGHT,
                    Draw.IsTypeThen(GridType.Floor, Draw.Count<GenSpace>(out side2)))))
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
                Container2D<GenSpace> copy = new Array2D<GenSpace>(spec.Room.Grids);
                copy.DrawLine(from, to, i, horizontal, Draw.SetToIfNotEqual(GridType.NULL, new GenSpace(GridType.INTERNAL_RESERVED_BLOCKED, spec.Theme)));
                copy.ToLog(Logs.LevelGen);
            }
        }
        #endregion

        // Draw selected splitter
        int picked = options.Random(spec.Random);
        List<Point> walls = new List<Point>();
        spec.Grids.DrawLine(from, to, picked, horizontal, Draw.Not(Draw.IsType<GenSpace>(GridType.NULL)).IfThen(Draw.SetTo(GridType.Wall, spec.Theme).And(Draw.AddTo<GenSpace>(walls))));
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Picked splitter:");
            spec.Room.ToLog(Logs.LevelGen);
        }
        #endregion

        spec.Grids.PlaceSomeDoors(walls, spec.Theme, spec.Random, 5);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Splitter Mod");
        }
        #endregion
        return true;
    }

    public override List<ProbabilityItem<RoomModifier>> GetChainedModifiers()
    {
        return new List<ProbabilityItem<RoomModifier>>(0);
    }
}
