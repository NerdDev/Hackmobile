using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GrandTombRoom : BaseRoomMod
{
    public override bool Modify(RoomSpec spec)
    {
        //spec.Grids
        return false;
    }

    public override List<ProbabilityItem<RoomModifier>> GetChainedModifiers()
    {
        return new List<ProbabilityItem<RoomModifier>>(0);
    }
}

