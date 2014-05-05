using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIDecisionCore
{
    List<AIDecision> decisions = new List<AIDecision>();

    public bool FillPool(ProbabilityPool<AIDecision> pool, AICore core, out AIDecision autoDecision)
    {
        foreach (AIDecision decision in decisions)
        {
            decision.Args.Reset();
            #region DEBUG
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
            {
                core.Log.printHeader(decision.GetType().Name);
            }
            #endregion
            if (decision.Decide(core))
            {
                #region DEBUG
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
                {
                    core.Log.w("Instant picking");
                    core.Log.printFooter(decision.GetType().Name);
                }
                #endregion
                autoDecision = decision;
                return true;
            }
            #region DEBUG
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
            {
                core.Log.printFooter(decision.GetType().Name);
            }
            #endregion
            pool.Add(decision, decision.Args.Weight);
        }
        autoDecision = null;
        return false;
    }

    public void AddDecision(AIDecision decision)
    {
        decisions.Add(decision);
    }
}