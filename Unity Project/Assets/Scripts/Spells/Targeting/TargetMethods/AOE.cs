using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Abstract definition to define all radius-based targeting methods
 */

public abstract class AOE : TargetedLocations
{
    private int radius = 1;
    public int Radius { get { return radius; } set { radius = value; } }
    public bool RequiresLOS;
    public bool ObjectsBlockLOS;
    public bool RequiresPathTo;
}
