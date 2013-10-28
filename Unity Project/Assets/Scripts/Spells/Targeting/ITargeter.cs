using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ITargeter
{
    TargetingStyle Style { get; }
    byte MaxTargets { get; set; }
    List<IAffectable> GetTargets(SpellCastInfo castInfo);
}
