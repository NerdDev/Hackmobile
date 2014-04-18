using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class TreasureRoomMod : HeavyRoomMod
{
    public override bool Unique { get { return true; } }

    protected override bool ModifyInternal(RoomSpec spec)
    {
        List<Bounding> options = spec.Grids.FindRectangles(5, 5, false, new StrokedAction<GenSpace>()
            {
                UnitAction = Draw.EmptyFloorNotBlocking(),
                StrokeAction = Draw.EmptyFloorNotBlocking().Or(Draw.WallType<GenSpace>())
            }, spec.Grids.Bounding);
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
        {
            BigBoss.Debug.w(Logs.LevelGen, options.Count + " Options: ");
            var save = new MultiMap<GenSpace>();
            var copy = new Array2D<GenSpace>(spec.Grids);
            foreach (Bounding r in options)
            {
                save.Clear();
                copy.DrawRect(r.XMin + 1, r.XMax - 1, r.YMin + 1, r.YMax - 1, Draw.AddTo(save).And(Draw.SetTo(GridType.Path_Vert, spec.Theme)));
                copy.ToLog(Logs.LevelGen);
                copy.PutAll(save);
            }
        }
        #endregion
        while (options.Count > 0)
        {
            Bounding bounding = options.RandomTake(spec.Random);
            Point center = bounding.GetCenter();
            if (spec.Grids[center].Type != GridType.Floor) return false;
            List<Point> doors = new List<Point>();
            HandleDoor(bounding.XMin, center.y, doors, spec);
            HandleDoor(bounding.XMin, center.y, doors, spec);
            HandleDoor(center.x, bounding.YMin, doors, spec);
            HandleDoor(center.x, bounding.YMax, doors, spec);
            if (doors.Count == 0) return false;
            // Draw it
            spec.Grids.DrawRect(bounding.XMin, bounding.XMax, bounding.YMin, bounding.YMax, new StrokedAction<GenSpace>()
            {
                StrokeAction = Draw.SetTo(GridType.Wall, spec.Theme),
                UnitAction = Draw.SetTo(GridType.Floor, spec.Theme)
            });
            foreach (var door in doors)
            {
                spec.Grids.SetTo(door, GridType.Door, spec.Theme);
            }
            spec.Grids.SetTo(center, GridType.Chest, spec.Theme);
            return true;
        }
        return true;
    }

    protected void HandleDoor(int x, int y, List<Point> doorOptions, RoomSpec spec)
    {
        if (spec.Grids.DrawDir(x, y, GridDirection.HORIZ, Draw.Walkable()))
        {
            doorOptions.Add(new Point(x, y));
        }
    }
}
