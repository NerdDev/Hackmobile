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

    public override double StickyShift
    {
        get { return 0; }
    }

    public override bool Decide(AICore core, out double weight, out DecisionActions actions)
    {
        weight = 0d;
        foreach (var pair in core.NPCMemory)
        {
            if (!pair.Value.Friendly && pair.Value.AwareOf)
            {
                actions = (coreP) => core.CurrentState = AIState.Combat;
                return true;
            }
        }
        actions = null;
        return false;
    }
}

