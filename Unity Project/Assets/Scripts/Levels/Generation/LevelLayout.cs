using System.Collections;
using System.Collections.Generic;
using System;

public class LevelLayout : LayoutObject
{
    public Bounding UpStart;
    public Bounding DownStart;
    public List<LayoutObject> Rooms = new List<LayoutObject>();
    public System.Random Random;

    public LevelLayout()
        : base("Level Layout")
    {

    }
}
