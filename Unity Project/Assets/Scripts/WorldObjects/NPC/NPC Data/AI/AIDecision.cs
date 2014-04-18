using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class AIDecision
{
    public abstract AIRole Role { get; }
    public int Cost { get; set; }

    public abstract void Action(AIActionArgs args);

    public abstract double CalcWeighting(AIDecisionArgs args);
}
