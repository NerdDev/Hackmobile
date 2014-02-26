using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GiantPillarMod : HeavyRoomMod
{
    public override RoomModType ModType { get { return RoomModType.Flexible; } }
    public override string Name { get { return "Giant Pillar"; } }

    public override bool Modify(RoomSpec spec)
    {
        int size = spec.Random.Next(3, 5);
        BigBoss.Debug.w(Logs.LevelGen, "Size: " + size);
        // Add an extra 2 for stroke width for analysis
        size += 2;
        List<Bounding> locations = spec.Grids.GetSquares(size, size, false, new StrokedAction<GenSpace>()
            {
                UnitAction = Draw.IsType<GenSpace>(GridType.Floor),
                StrokeAction = Draw.Walkable<GenSpace>()
            },
            spec.Room.GetBounding(false));
        if (locations.Count == 0) return false;
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
        {
            BigBoss.Debug.w(Logs.LevelGen, locations.Count + " Options: ");
            var save = new MultiMap<GenSpace>();
            var copy = new Array2D<GenSpace>(spec.Room.Grids);
            foreach (Bounding r in locations)
            {
                save.Clear();
                copy.DrawSquare(r.XMin + 1, r.XMax - 1, r.YMin + 1, r.YMax - 1, Draw.AddTo(save).And(Draw.SetTo(new GenSpace(GridType.Path_Vert, spec.Theme))));
                copy.ToLog(Logs.LevelGen);
                copy.PutAll(save);
            }
        }
        #endregion
        Bounding l = locations.Random(spec.Random);
        // Draw inner square without stroke (stroke was just used to analyze surroundings)
        spec.Grids.DrawSquare(l.XMin + 1, l.XMax - 1, l.YMin + 1, l.YMax - 1, Draw.SetTo(new GenSpace(GridType.Wall, spec.Theme)));
        return true;
    }
}
