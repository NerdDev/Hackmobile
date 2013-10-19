using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class AOE : TargetedLocation
{
    private int radius = 1;
    public int Radius { get { return radius; } set { radius = value; } }
    public bool RequiresLOS { get; set; }
    public bool ObjectsBlockLOS { get; set; }
    public bool RequiresPathTo { get; set; }
}
