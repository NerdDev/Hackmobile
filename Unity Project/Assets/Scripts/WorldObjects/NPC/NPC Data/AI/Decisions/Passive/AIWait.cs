using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIWait : AIDecision
{
    public override double StickyShift { get { return 0d; } }

    public override double Cost { get { return 60d; } }

    public override void Action(AICore core)
    {
        // Nuttin'
    }

    public override bool CalcWeighting(AICore core, out double weight)
    {
        weight = 1d;
        return false;
    }
}
