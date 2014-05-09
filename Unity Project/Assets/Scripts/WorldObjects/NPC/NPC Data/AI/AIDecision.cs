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

    public bool PassControl(AICore core, AIDecisionCore decisionCore, AIDecision decision, out bool instantPick)
    {
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            core.Log.printHeader(decision.GetType().ToString());
        }
        instantPick = decision.Decide(core, decisionCore);
        Args.CopyIn(decision.Args);
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            core.Log.printFooter(decision.GetType().ToString());
        }
        return instantPick || !decision.Args.Weight.EqualsWithin(0d);
    }

    public virtual void ParseXML(XMLNode x)
    {
    }

    public override bool Equals(object obj)
    {
        AIDecision rhs = obj as AIDecision;
        if (rhs == null) return false;
        return GetType().Equals(rhs.GetType());
    }

    public override int GetHashCode()
    {
        return GetType().GetHashCode();
    }
}
