using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class AIDecision : IXmlParsable
{
    public abstract double Cost { get; }

    public abstract double StickyShift { get; }

    public DecisionActions Actions;

    public abstract IEnumerable<AIState> States { get; }

    public abstract bool Decide(AICore core, out double weight, out DecisionActions actions);

    public virtual void ParseXML(XMLNode x)
    {
    }
}
