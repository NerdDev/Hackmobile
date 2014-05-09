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

    public void Reset()
    {
        Weight = 1;
        StickyShift = 0;
        StickyReduc = 0;
        Ending = false;
    }

    public void CopyIn(AIDecisionArgs rhs)
    {
        Weight = rhs.Weight;
        StickyShift = rhs.StickyShift;
        StickyReduc = rhs.StickyReduc;
        Ending = rhs.Ending;
        Actions = rhs.Actions;
    }

    public void ToLog(Log log)
    {
        log.w("Weight: " + Weight + ", StickyShift: " + StickyShift + ", StickyReduc: " + StickyReduc + ", Ending: " + Ending);
    }
}

