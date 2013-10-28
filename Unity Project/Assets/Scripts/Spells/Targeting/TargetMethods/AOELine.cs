using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AOELine : AOE
{
    public bool IncludeEnds { get; set; }
    public override List<IAffectable> GetTargets(SpellCastInfo castInfo)
    {
        // Logic to do line aoe targeting
        return base.GetTargets(castInfo);
    }
}
