using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIAggro : AIDecision
{
    public override double Cost
    {
        get { return 0; }
    }

    public override double StickyShift
    {
        get { return 0; }
    }

    public override void Action(AICore core)
    {
        core.CurrentState = AIState.Combat;
    }

    public override bool CalcWeighting(AICore core, out double weight)
    {
        weight = 0d;
        foreach (var pair in core.NPCMemory)
        {
            if (!pair.Value.Friendly && pair.Value.AwareOf)
            {
                return true;
            }
        }
        return false;
    }
}

