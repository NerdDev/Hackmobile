using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UndeadTombTheme : Theme
{
    public override void Init()
    {
        base.Init();
        // Base
        RoomMods.AddMod(new CircleRoom(), .5d);
        RoomMods.AddMod(new RectangularRoom(), 2d);
        RoomMods.AddMod(new SquareRoom(), 1d);
        RoomMods.AddMod(new TRoomMod(), 1d);
        // Defining
        RoomMods.AddMod(new BlankDefiningRoomMod(), 1);
        //RoomMods.AddMod(new GrandTombRoom(), 2, true);
        // Flex
        RoomMods.AddMod(new MassTombRoom(), 1, true);
        RoomMods.AddMod(new GiantPillarMod(), 1);
        RoomMods.AddMod(new HiddenRoomMod(), .5);
        RoomMods.AddMod(new PillarMod(), 1);
        RoomMods.AddMod(new SplitterMod(), 1);
        RoomMods.AddMod(new TrapRoomMod(), .2d);
        RoomMods.AddMod(new TreasureRoomMod(), .3d);
    }
}

