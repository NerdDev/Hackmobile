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
            if (decision.CalcWeighting(core, out weight))
            {
                autoDecision = decision;
                return true;
            }
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