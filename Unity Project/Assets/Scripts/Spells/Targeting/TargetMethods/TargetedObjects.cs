using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Targeter meant to target only selected objects
 */
public class TargetedObjects : Targeter
{
    public TargetingStyle Style { get { return TargetingStyle.TargetObject; } }
    public byte MaxTargets { get; set; }
    public override HashSet<IAffectable> GetAffectableTargets(SpellCastInfo castInfo)
    {
        return new HashSet<IAffectable>(castInfo.TargetObjects);
    }

    public int GetHash()
    {
        int hash = 5;
        hash += Style.GetHashCode() * 13;
        hash += MaxTargets.GetHashCode() * 3;
        return base.GetHash() + hash;
    }
}
