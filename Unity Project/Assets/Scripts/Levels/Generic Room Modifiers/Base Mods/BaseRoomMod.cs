using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class BaseRoomMod : RoomModifier
{
    public double Scale;

    public BaseRoomMod()
    {
        Scale = 1d;
    }
}

