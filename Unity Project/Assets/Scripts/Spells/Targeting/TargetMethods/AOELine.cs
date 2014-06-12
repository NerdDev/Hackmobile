using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Targeter meant to target everyone in a line (radius gives it width)
 */

public class AOELine : AOE
{
    public bool IncludeCaster;
    public bool IncludeEndPoint;
    public override HashSet<IAffectable> GetAffectableTargets(SpellCastInfo castInfo)
    {
        // Logic to draw line in any direction
        return base.GetAffectableTargets(castInfo);
    }
}
