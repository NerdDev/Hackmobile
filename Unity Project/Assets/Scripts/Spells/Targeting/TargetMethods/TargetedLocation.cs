using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TargetedLocation : ITargeter
{
    public TargetingStyle Style { get { return TargetingStyle.TargetLocation; } }
    public byte MaxTargets { get; set; }
    public virtual List<IAffectable> GetTargets(SpellCastInfo castInfo)
    {
        List<IAffectable> ret = new List<IAffectable>();
        foreach (GridSpace space in castInfo.TargetSpaces)
            foreach (WorldObject obj in space.GetContained())
                if (obj is IAffectable)
                    ret.Add((IAffectable)obj);
        return ret;
    }
}
