using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TargetedObject : ITargeter
{
    public TargetingStyle Style { get { return TargetingStyle.TargetObject; } }
    public byte MaxTargets { get; set; }
    public List<IAffectable> GetTargets(SpellCastInfo castInfo)
    {
        return castInfo.TargetObjects.ToList();
    }
}
