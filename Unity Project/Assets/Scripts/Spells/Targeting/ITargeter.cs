using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ITargeter
{
    TargetingStyle Style { get; }
    byte MaxTargets { get; set; }
    HashSet<IAffectable> GetTargets(SpellCastInfo castInfo);

    int GetHash();
}
