using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIAggro : AIDecision
{
    public override IEnumerable<AIState> States { get { yield return AIState.Passive; } }

    public override double Cost
    {
        get { return 0; }
    }

    public override bool Decide(AICore core, AIDecisionCore decisionCore)
    {
        Args.Weight = 0d;
        foreach (var pair in core.NPCMemory)
        {
            if (!pair.Value.Friendly && pair.Value.AwareOf)
            {
                Args.Actions = (coreP) => core.CurrentState = AIState.Combat;
                Args.Weight = double.PositiveInfinity;
                return true;
            }
        }
        return false;
    }
}

