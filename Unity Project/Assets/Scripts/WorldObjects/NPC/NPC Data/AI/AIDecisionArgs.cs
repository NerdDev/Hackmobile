using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIDecisionArgs
{
    private AICore core;
    public NPC Self { get { return core.NPC; } }
    public AIState CurrentState { get { return core.CurrentState; } }
    public NPC Target;
    public Func<double, double> WeightingCurve;

    public AIDecisionArgs(AICore core)
    {
        this.core = core;
    }
}
