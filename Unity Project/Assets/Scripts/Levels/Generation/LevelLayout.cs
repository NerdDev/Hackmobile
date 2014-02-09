using System.Collections;
using System.Collections.Generic;
using System;

public class LevelLayout : LayoutObjectContainer
{
    public Point UpStart;
    public Point DownStart;
    public List<LayoutObject> Rooms = new List<LayoutObject>();
}
