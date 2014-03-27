using System.Collections;
using System.Collections.Generic;
using System;

public class LevelLayout : LayoutObject
{
    public Boxing UpStart;
    public Boxing DownStart;
    public List<LayoutObject> Rooms = new List<LayoutObject>();
    public System.Random Random;

    public LevelLayout()
        : base("Level Layout")
    {

    }
}
