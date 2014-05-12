using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIDecisionArgs
{
    public double Weight;
    public double StickyShift;
    public double StickyReduc;
    public bool Ending;
    public DecisionActions Actions;
    public AIDecision PassedDecision;
    public AIDecision LastPassedDecision;

    public void Reset()
    {
        Weight = 1;
        StickyShift = 0;
        StickyReduc = 0;
        Ending = false;
        LastPassedDecision = PassedDecision;
        PassedDecision = null;
    }

    public void PassTo(AIDecision rhsDecision)
    {
        Weight = rhsDecision.Args.Weight;
        StickyShift = rhsDecision.Args.StickyShift;
        StickyReduc = rhsDecision.Args.StickyReduc;
        Ending = rhsDecision.Args.Ending;
        Actions = rhsDecision.Args.Actions;
        PassedDecision = rhsDecision;
    }

    public void ToLog(Log log)
    {
        log.w("Weight: " + Weight + ", StickyShift: " + StickyShift + ", StickyReduc: " + StickyReduc + ", Ending: " + Ending);
    }
}

