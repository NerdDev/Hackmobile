using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Targeter meant to target only selected objects
 */
public class TargetedObjects : ITargeter
{
    public TargetingStyle Style { get { return TargetingStyle.TargetObject; } }
    public byte MaxTargets { get; set; }
    public HashSet<IAffectable> GetTargets(SpellCastInfo castInfo)
    {
        return new HashSet<IAffectable>(castInfo.TargetObjects);
    }
}
