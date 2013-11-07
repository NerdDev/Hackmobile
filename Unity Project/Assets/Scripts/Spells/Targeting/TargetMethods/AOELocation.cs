using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AOELocation : AOE
{
    public override HashSet<IAffectable> GetTargets(SpellCastInfo castInfo)
    {
        HashSet<GridSpace> targetSpaces = new HashSet<GridSpace>();
        GridSpace centerSpace = castInfo.TargetSpace;

        return base.GetTargets(castInfo);
    }
}
