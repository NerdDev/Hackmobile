using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BlankDefiningRoomMod : DefiningRoomMod
{
    public override bool Modify(RoomSpec spec)
    {
        return true;
    }
}

