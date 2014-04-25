using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIDecisionCore
{
    List<AIDecision> decisions = new List<AIDecision>();

    public void FillPool(ProbabilityPool<AIDecision> pool, AICore core)
    {
        foreach (AIDecision decision in decisions)
        {
            double weight = core.WeightingCurve(decision);
            pool.Add(decision, weight);
        }
    }

    public void AddDecision(AIDecision decision)
    {
        decisions.Add(decision);
    }
}