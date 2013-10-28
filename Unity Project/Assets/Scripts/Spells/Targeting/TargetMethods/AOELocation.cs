using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AOELocation : AOE
{
    public virtual List<IAffectable> GetTargets(SpellCastInfo castInfo)
    {
        // Logic to get AOE locations in a circle
        return base.GetTargets(castInfo);
    }
}
