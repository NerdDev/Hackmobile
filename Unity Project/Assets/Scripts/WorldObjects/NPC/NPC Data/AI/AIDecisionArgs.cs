using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIDecisionArgs
{
    public double Weight;
    public double Sticky;
    public DecisionActions Actions;

    public void Reset()
    {
        Weight = 1;
        Sticky = 0;
    }
}

