using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class AIUseAbility : AIDecision
{
    public override double Cost { get { return 60d; } }
    public override double StickyShift { get { return 0d; } }

    public AIUseAbility()
    {
    }

    public override void Action(AICore core)
    {
        // Code to roll between damage options
        
        // Temporarily just doing autoattack for now.
        if (core.Self.IsNextToTarget(core.Target))
        {
            core.Self.attack(core.Target);
        }
        else
        {
            core.MoveTo(core.Target);
        }
    }

    public override double CalcWeighting(AICore core)
    {
        // Do Damage is the arbitrary in-combat standard of 1
        return 1d;
    }
}
