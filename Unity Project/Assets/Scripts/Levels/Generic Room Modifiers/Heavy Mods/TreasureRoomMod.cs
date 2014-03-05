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
                UnitAction = Draw.Walkable<GenSpace>(),
                StrokeAction = Draw.Not(Draw.Blocking(Draw.Walkable<GenSpace>())).And(Draw.IsType<GenSpace>(GridType.Floor).Or(Draw.WallType<GenSpace>()))
            }, spec.Grids.Bounding);
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
                StrokeAction = Draw.SetTo<GenSpace>(new GenSpace(GridType.Wall, spec.Theme)),
                UnitAction = Draw.SetTo<GenSpace>(new GenSpace(GridType.Floor, spec.Theme))
            });
            foreach (var door in doors)
            {
                spec.Grids[door] = new GenSpace(GridType.Door, spec.Theme);
            }
            spec.Grids[center] = new GenSpace(GridType.Chest, spec.Theme);
            return true;
        }
        return true;
    }

    protected void HandleDoor(int x, int y, List<Point> doorOptions, RoomSpec spec)
    {
        if (spec.Grids.DrawDir(x, y, GridDirection.HORIZ, Draw.Walkable<GenSpace>()))
        {
            doorOptions.Add(new Point(x, y));
        }
    }
}
