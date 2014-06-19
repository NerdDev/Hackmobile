using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class BaseRoomMod : RoomModifier
{
    public override RoomModifierType Type
    {
        get { return RoomModifierType.Base; }
    }

    public BaseRoomMod()
    {
    }

    public bool Modify(RoomSpec spec, double scale)
    {
        return ModifyInternal(spec, scale);
    }

    protected override bool ModifyInternal(RoomSpec spec)
    {
        return ModifyInternal(spec, 1d);
    }

    protected abstract bool ModifyInternal(RoomSpec spec, double scale);
}

