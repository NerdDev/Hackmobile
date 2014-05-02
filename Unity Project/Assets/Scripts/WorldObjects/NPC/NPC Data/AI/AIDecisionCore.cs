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
            double weight;
            DecisionActions actions;
            if (decision.Decide(core, out weight, out actions))
            {
                decision.Actions = actions;
                autoDecision = decision;
                return true;
            }
            decision.Actions = actions;
            pool.Add(decision, weight);
        }
        autoDecision = null;
        return false;
    }

    public void AddDecision(AIDecision decision)
    {
        decisions.Add(decision);
    }
}