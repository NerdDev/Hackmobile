using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class BaseRoomMod : RoomModifier
{
    public virtual int MinFlexMods { get { return 3; } }
    public virtual int MaxFlexMods { get { return 6; } }
}

