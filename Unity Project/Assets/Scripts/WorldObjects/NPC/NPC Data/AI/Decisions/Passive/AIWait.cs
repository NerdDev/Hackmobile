using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIWait : AIRoleDecision
{
    public override AIRole Role { get { return AIRole.Other; } }
    public override double StickyShift { get { return 0d; } }

    public override double Cost { get { return 60d; } }

    public override void Action(AIActionArgs args)
    {
        // Nuttin'
    }

    public override double CalcWeighting(AIDecisionArgs args)
    {
        return 1d;
    }
}
