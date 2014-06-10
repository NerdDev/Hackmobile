using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/*
 * Targeter meant to target every object in given spaces
 */

public class TargetedLocations : Targeter
{
    public TargetingStyle Style { get { return TargetingStyle.TargetGrid; } }
    public byte MaxTargets { get; set; }

    public virtual HashSet<Vector3> GetLocationTargets(SpellCastInfo castInfo)
    {
        return new HashSet<Vector3>(castInfo.TargetLocations);
    }

    public int GetHash()
    {
        int hash = 5;
        hash += Style.GetHashCode() * 13;
        hash += MaxTargets.GetHashCode() * 3;
        return hash;
    }
}
