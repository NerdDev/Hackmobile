using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Self : ITargeter
{
    public TargetingStyle Style { get { return TargetingStyle.Self; } }
    public byte MaxTargets { get { return 0; } set { } }
    public HashSet<IAffectable> GetTargets(SpellCastInfo castInfo)
    {
        return new HashSet<IAffectable> {castInfo.Caster};
    }

    public int GetHash()
    {
        int hash = 5;
        hash += Style.GetHashCode() * 13;
        hash += MaxTargets.GetHashCode() * 3;
        return hash;
    }
}
