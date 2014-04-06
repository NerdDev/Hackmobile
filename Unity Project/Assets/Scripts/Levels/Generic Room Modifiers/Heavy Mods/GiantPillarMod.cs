using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GiantPillarMod : HeavyRoomMod
{
    protected override bool ModifyInternal(RoomSpec spec)
    {
        int size = spec.Random.Next(3, 5);
        BigBoss.Debug.w(Logs.LevelGen, "Size: " + size);
        // Add an extra 2 for stroke width for analysis
        size += 2;
        List<Bounding> locations = spec.Grids.FindRectangles(size, size, false, new StrokedAction<GenSpace>()
            {
                UnitAction = Draw.Or(Draw.IsType<GenSpace>(GridType.Floor), Draw.IsType<GenSpace>(GridType.Wall)).And(Draw.Empty()),
                StrokeAction = Draw.Walkable()
            },
            spec.Grids.Bounding);
        if (locations.Count == 0) return false;
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
        {
            BigBoss.Debug.w(Logs.LevelGen, locations.Count + " Options: ");
            var save = new MultiMap<GenSpace>();
            var copy = new Array2D<GenSpace>(spec.Grids);
            foreach (Bounding r in locations)
            {
                save.Clear();
                copy.DrawRect(r.XMin + 1, r.XMax - 1, r.YMin + 1, r.YMax - 1, Draw.AddTo(save).And(Draw.SetTo(GridType.Path_Vert, spec.Theme)));
                copy.ToLog(Logs.LevelGen);
                copy.PutAll(save);
            }
        }
        #endregion
        Bounding l = locations.Random(spec.Random);
        // Draw inner square without stroke (stroke was just used to analyze surroundings)
        spec.Grids.DrawRect(l.XMin + 1, l.XMax - 1, l.YMin + 1, l.YMax - 1, Draw.SetTo(GridType.Wall, spec.Theme));
        return true;
    }
}
