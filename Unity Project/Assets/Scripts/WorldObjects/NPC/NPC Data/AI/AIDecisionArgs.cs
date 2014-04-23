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
    public Func<AIDecision, double> WeightingCurve;
    public System.Random Random { get { return core.Random; } }
    public AIDecision LastDecision { get { return core.LastDecision; } }
    public AIDecision CurrentDecision;
    public bool Continuing { get { return Object.ReferenceEquals(LastDecision, CurrentDecision); } }

    public AIDecisionArgs(AICore core)
    {
        this.core = core;
    }
}
