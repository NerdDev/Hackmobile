using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class TreasureRoomMod : HeavyRoomMod
{
    public override string Name { get { return "Treasure Room"; } }
    public override bool Unique { get { return true; } }

    public override bool Modify(RoomSpec spec)
    {
        LayoutObject room = spec.Room;
        Bounding bounds = room.GetBounding(true);
        int centerX = (bounds.XMin + bounds.XMax) / 2;
        int centerY = (bounds.YMin + bounds.YMax) / 2;
        room.Grids.DrawRect(centerX, centerY, 2, new StrokedAction<GenSpace>()
        {
            StrokeAction = Draw.SetTo<GenSpace>(new GenSpace(GridType.Wall, spec.Theme)),
            UnitAction = Draw.SetTo<GenSpace>(new GenSpace(GridType.Floor, spec.Theme))
        });
        room.Grids.SetTo(centerX, centerY + 2, new GenSpace(GridType.Door, spec.Theme));
        room.Grids.SetTo(centerX, centerY - 2, new GenSpace(GridType.Door, spec.Theme));
        room.Grids.SetTo(centerX + 2, centerY, new GenSpace(GridType.Door, spec.Theme));
        room.Grids.SetTo(centerX - 2, centerY, new GenSpace(GridType.Door, spec.Theme));
        room.Grids.SetTo(centerX, centerY, new GenSpace(GridType.Chest, spec.Theme));
        return true;
    }
}
