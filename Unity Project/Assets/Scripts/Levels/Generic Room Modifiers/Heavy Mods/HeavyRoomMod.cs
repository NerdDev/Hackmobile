﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class HeavyRoomMod : RoomModifier
{
    public override RoomModifierType Type
    {
        get { return RoomModifierType.Heavy; }
    }
}

