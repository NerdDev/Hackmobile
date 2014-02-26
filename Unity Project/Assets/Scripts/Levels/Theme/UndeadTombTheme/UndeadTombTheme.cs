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
        AddMod(new CircleRoom(), .5d);
        AddMod(new RectangularRoom(), 2d);
        AddMod(new SquareRoom(), 1d);
        AddMod(new GiantPillarMod(), 1);
        AddMod(new HiddenRoomMod(), .5);
        AddMod(new PillarMod(), 1);
        AddMod(new SplitterMod(), 1);
        AddMod(new TrapRoomMod(), .2d);
        AddMod(new TreasureRoomMod(), .3d);
    }
}

