using System.Collections;
using System.Collections.Generic;
using System;

public class LevelLayout : LayoutObject<GenSpace>
{
    public List<LayoutObject<GenSpace>> Rooms = new List<LayoutObject<GenSpace>>();
    public System.Random Random;

    public LevelLayout()
        : base("Level Layout")
    {

    }
}
