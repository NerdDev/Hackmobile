using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIWait : AIDecision
{
    public override double StickyShift { get { return 0d; } }
    public override IEnumerable<AIState> States { get { yield return AIState.Passive; yield return AIState.Combat; } }

    public override double Cost { get { return 60d; } }

    public override bool Decide(AICore core, out double weight, out DecisionActions actions)
    {
        if (core.CurrentState == AIState.Passive)
        {
            weight = 1d;
        }
        else
        { // Last resort
            weight = 0.05d;
        }
        actions = null;
        return false;
    }
}
