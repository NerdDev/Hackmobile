using System.Collections;
using System.Collections.Generic;
using System;

public class LevelLayout : LayoutObject<GenSpace>
{
    public System.Random Random;

    public LevelLayout()
        : base(LayoutObjectType.Layout)
    {
    }
}
