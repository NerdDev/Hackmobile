using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Self : ITargeter
{
    public TargetingStyle Style { get { return TargetingStyle.Self; } }
    public byte MaxTargets { get { return 0; } set { } }
    public List<IAffectable> GetTargets(SpellCastInfo castInfo)
    {
        List<IAffectable> ret = new List<IAffectable>();
        ret.Add(castInfo.Caster);
        return ret;
    }
}
