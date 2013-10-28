using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AOETargeted : AOELocation
{
    public override List<IAffectable> GetTargets(SpellCastInfo castInfo)
    {
        // Force spaces to be derived
        castInfo.TargetSpaces = null;
        return base.GetTargets(castInfo);
    }
}
