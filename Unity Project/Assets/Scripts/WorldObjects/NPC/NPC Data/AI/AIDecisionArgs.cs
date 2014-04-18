using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIDecisionArgs
{
    private AICore core;
    public NPC NPC { get { return core.NPC; } }
    public Func<double, double> WeightingCurve;

    public AIDecisionArgs(AICore core)
    {
        this.core = core;
    }
}
