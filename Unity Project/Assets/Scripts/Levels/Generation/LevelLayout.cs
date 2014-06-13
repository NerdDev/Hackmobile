using System.Collections;
using System.Collections.Generic;
using System;

public class LevelLayout : LayoutObject<GenSpace>
{
    public System.Random Random;
    public Dictionary<Theme, List<LayoutObject<GenSpace>>> RoomsByTheme;

    public LevelLayout()
        : base(LayoutObjectType.Layout)
    {
    }
}
