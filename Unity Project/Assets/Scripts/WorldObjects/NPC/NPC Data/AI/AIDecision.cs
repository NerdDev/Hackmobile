using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class AIDecision
{
    public abstract double Cost { get; }

    public abstract double StickyShift { get; }

    public abstract void Action(AIActionArgs args);

    public abstract double CalcWeighting(AIDecisionArgs args);
}
