using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class AIDecision : AIAction
{
    public abstract double CalcWeighting(AIDecisionArgs args);
}
