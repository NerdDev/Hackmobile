using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AOESelf : AOETargeted
{
    public override List<IAffectable> GetTargets(SpellCastInfo castInfo)
    {
        // Confirm target is caster
        castInfo.TargetObject = castInfo.Caster;
        return base.GetTargets(castInfo);
    }
}