using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Targeter meant to target only selected objects
 */
public class TargetedObjects : Targeter
{
    public override TargetingStyle Style { get { return TargetingStyle.TargetObject; } }
    public override byte MaxTargets { get { return 1; } set { } }
    public override HashSet<IAffectable> GetAffectableTargets(SpellCastInfo castInfo)
    {
        HashSet<IAffectable> set = new HashSet<IAffectable>(castInfo.TargetObjects);
        return set;
    }

    public override int GetHash()
    {
        int hash = 5;
        hash += Style.GetHashCode() * 13;
        hash += MaxTargets.GetHashCode() * 3;
        return base.GetHash() + hash;
    }

    public override void ParseXML(XMLNode x)
    {
        base.ParseXML(x);
    }
}
