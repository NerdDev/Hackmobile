using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Targeter meant to target everyone around caster
 */
public class AOESelf : AOETargeted
{
    public override HashSet<IAffectable> GetTargets(SpellCastInfo castInfo)
    {
        // Confirm target is caster
        castInfo.TargetObject = castInfo.Caster;
        return base.GetTargets(castInfo);
    }
}
