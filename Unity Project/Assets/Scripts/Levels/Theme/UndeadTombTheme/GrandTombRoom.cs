﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GrandTombRoom : BaseRoomMod
{
    public override bool Modify(RoomSpec spec)
    {
        int numTombs = spec.Random.Next(2, 5) * 2;
        //bool 
        //List<Bounding> largestRects = spec.Grids.FindLargestRectangles(false, )
        return false;
    }

    public override List<ProbabilityItem<RoomModifier>> GetChainedModifiers()
    {
        return new List<ProbabilityItem<RoomModifier>>(0);
    }
}

