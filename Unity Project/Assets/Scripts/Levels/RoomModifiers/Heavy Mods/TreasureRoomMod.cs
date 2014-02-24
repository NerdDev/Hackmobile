using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class TreasureRoomMod : HeavyRoomMod
{
    public override RoomModType ModType { get { return RoomModType.Flexible; } }
    public override string Name { get { return "Treasure Room"; } }
    public override bool Unique { get { return true; } }

    public override bool Modify(RoomSpec spec)
    {
        LayoutObject room = spec.Room;
        Bounding bounds = room.GetBounding(true);
        int centerX = (bounds.XMin + bounds.XMax) / 2;
        int centerY = (bounds.YMin + bounds.YMax) / 2;
        room.Grids.DrawSquare(centerX, centerY, 2, new StrokedAction<GridSpace>()
        {
            StrokeAction = Draw.SetTo(GridType.Wall),
            UnitAction = Draw.SetTo(GridType.Floor)
        });
        room.Grids.SetTo(centerX, centerY + 2, GridType.Door);
        room.Grids.SetTo(centerX, centerY - 2, GridType.Door);
        room.Grids.SetTo(centerX + 2, centerY, GridType.Door);
        room.Grids.SetTo(centerX - 2, centerY, GridType.Door);
        room.Grids.SetTo(centerX, centerY, GridType.Chest);
        return true;
    }
}
