using System.Collections;
using System.Collections.Generic;
using System;

public class LevelLayout : LayoutObject<GenSpace>
{
    public LayoutObjectContainer<GenSpace> AllContainer = new LayoutObjectContainer<GenSpace>();
    public LayoutObjectContainer<GenSpace> RoomContainer = new LayoutObjectContainer<GenSpace>();
    public System.Random Random;

    public LevelLayout()
        : base("Level Layout")
    {
    }
}
