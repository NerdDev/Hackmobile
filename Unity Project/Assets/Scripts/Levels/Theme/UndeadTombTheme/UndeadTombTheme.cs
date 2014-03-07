using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UndeadTombTheme : Theme, IPillarTheme
{
    public ThemeElementCollection[] Pillars;
    public ThemeElementCollection[] Tombs;

    public override void Init()
    {
        base.Init();
        // Base
        _roomMods.AddMod(new CircleRoom(), .5d);
        _roomMods.AddMod(new RectangularRoom(), 2d);
        _roomMods.AddMod(new SquareRoom(), 1d);
        _roomMods.AddMod(new TRoomMod(), 1d);
        //_roomMods.AddMod(new GrandTombRoom(), 0.5d, true);
        // Defining
        _roomMods.AddMod(new BlankDefiningRoomMod(), 1);
        // Flex
        _roomMods.AddMod(new MassTombRoom(), 1, true);
        _roomMods.AddMod(new GiantPillarMod(), 1);
        _roomMods.AddMod(new HiddenRoomMod(), .5);
        _roomMods.AddMod(new PillarMod(), 1);
        _roomMods.AddMod(new SplitterMod(), 1);
        _roomMods.AddMod(new TrapRoomMod(), .2d);
        _roomMods.AddMod(new TreasureRoomMod(), .3d);
    }

    public ThemeElementCollection[] GetPillars()
    {
        return Pillars;
    }
}
