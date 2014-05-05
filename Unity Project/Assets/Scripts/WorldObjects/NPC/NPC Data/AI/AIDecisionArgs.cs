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
}

