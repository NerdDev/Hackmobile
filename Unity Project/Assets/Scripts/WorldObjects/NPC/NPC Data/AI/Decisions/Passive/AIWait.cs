using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIWait : AIDecision
{
    public override IEnumerable<AIState> States { get { yield return AIState.Passive; yield return AIState.Combat; } }

    public override double Cost { get { return 60d; } }

    public override bool Decide(AICore core, AIDecisionCore decisionCore)
    {
        if (core.CurrentState == AIState.Passive)
        {
            Args.Weight = 1d;
        }
        else
        { // Last resort
            Args.Weight = 0.05d;
        }
        return true;
    }
}
