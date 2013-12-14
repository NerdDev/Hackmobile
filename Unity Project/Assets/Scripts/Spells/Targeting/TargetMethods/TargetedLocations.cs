using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * Targeter meant to target every object in given spaces
 */

public class TargetedLocations : ITargeter
{
    public TargetingStyle Style { get { return TargetingStyle.TargetLocation; } }
    public byte MaxTargets { get; set; }
    public virtual HashSet<IAffectable> GetTargets(SpellCastInfo castInfo)
    {
        var ret = new HashSet<IAffectable>();
        foreach (GridSpace space in castInfo.TargetSpaces)
            foreach (WorldObject obj in space.GetContained())
            {
                var item = obj as IAffectable;
                if (item != null)
                    ret.Add(item);
            }
        return ret;
    }
}
