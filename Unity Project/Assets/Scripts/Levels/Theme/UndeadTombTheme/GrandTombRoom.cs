using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GrandTombRoom : BaseRoomMod
{
    protected override bool ModifyInternal(RoomSpec spec, double scale)
    {
        spec.RoomModifiers.RemoveMod(this);
        BaseRoomMod baseMod = spec.RoomModifiers.BaseMods.Get(spec.Random);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Picked base mod " + baseMod);
        }
        #endregion
        scale *= 2d;
        baseMod.Modify(spec, scale);
        int numTombs = spec.Random.Next(2, 5) * 2;
        List<Bounding> largestRects = spec.Grids.FindLargestRectangles(false, new StrokedAction<GenSpace>()
        {
            UnitAction = Draw.IsType<GenSpace>(GridType.Floor)
        }, spec.Grids.Bounding);
        if (largestRects.Count == 0) return false;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Largest Rect Options");
            foreach (Bounding bounds in largestRects)
            {
                MultiMap<GenSpace> tmp = new MultiMap<GenSpace>(spec.Grids);
                tmp.DrawRect(bounds, new StrokedAction<GenSpace>()
                    {
                        UnitAction = Draw.SetTo(new GenSpace(GridType.INTERNAL_RESERVED_BLOCKED, spec.Theme))
                    });
                tmp.ToLog(Logs.LevelGen);
            }
        }
        #endregion
        Bounding bound = largestRects.Random(spec.Random);
        // Place tombs

        int buffer = spec.Random.NextBool() ? 1 : 2;
        int pillarSize = 3;
        // Place pillars
        spec.Grids.DrawRect(bound.XMin + buffer, bound.XMin + pillarSize + buffer - 1, bound.YMin + buffer, bound.YMin + pillarSize + buffer - 1, Draw.SetTo(GridType.Pillar, spec.Theme));
        //spec.Grids.DrawRect(bound.XMax - 3, bound.XMin + 3, bound.YMin, bound.YMin + 3, Draw.SetTo(GridType.Pillar, spec.Theme));
        //spec.Grids.DrawRect(bound.XMin, bound.XMin + 3, bound.YMin, bound.YMin + 3, Draw.SetTo(GridType.Pillar, spec.Theme));
        //spec.Grids.DrawRect(bound.XMin, bound.XMin + 3, bound.YMin, bound.YMin + 3, Draw.SetTo(GridType.Pillar, spec.Theme));
        // Remove more tomb mods
        spec.RoomModifiers.RemoveMod(new MassTombRoom(), true);
        return false;
    }
}

