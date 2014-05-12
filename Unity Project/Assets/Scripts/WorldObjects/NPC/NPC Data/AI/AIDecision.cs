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

    public bool PassControl(AICore core, AIDecisionCore decisionCore, AIDecision decision)
    {
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            core.Log.printHeader(decision.GetType().ToString());
        }
        decision.Args.Reset();
        if (decision.Decide(core, decisionCore))
        {
            Args.PassTo(decision);
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
            {
                core.Log.printFooter(decision.GetType().ToString());
            }
            return true;
        }
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            core.Log.printFooter(decision.GetType().ToString());
        }
        return false;
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
