using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Targetter meant to target everyone in a radius around given objects
 */
public class AOETargeted : AOELocation
{
    public override HashSet<IAffectable> GetTargets(SpellCastInfo castInfo)
    {
        // Force spaces to be derived and then call AOE location's code
        castInfo.TargetSpaces = null;
        return base.GetTargets(castInfo);
    }
}
