using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class AIDecision : IXmlParsable
{
    public abstract double Cost { get; }

    public AIDecisionArgs Args = new AIDecisionArgs();

    public abstract IEnumerable<AIState> States { get; }

    public abstract bool Decide(AICore core, AIDecisionCore decisionCore);

    public virtual void ParseXML(XMLNode x)
    {
    }
}
