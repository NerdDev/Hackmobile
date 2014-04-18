using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIAggro : AIDecision
{
    public override AIRole Role { get { return AIRole.Other; } }

    public override void Action(AIActionArgs args)
    {
        throw new NotImplementedException();
    }

    public override double CalcWeighting(AIDecisionArgs args)
    {
        throw new NotImplementedException();
    }
}
