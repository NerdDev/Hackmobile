using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BlankDefiningRoomMod : DefiningRoomMod
{
    protected override bool ModifyInternal(RoomSpec spec)
    {
        return true;
    }
}

