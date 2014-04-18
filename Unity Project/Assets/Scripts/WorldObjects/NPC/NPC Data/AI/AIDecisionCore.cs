using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIDecisionCore
{
    List<AIDecision> decisions = new List<AIDecision>();

    public AIDecisionCore()
    {
    }

    public void FillPool(ProbabilityPool<AIDecision> pool, AIDecisionArgs args)
    {
        foreach (AIDecision decision in decisions)
        {
            double weight = decision.CalcWeighting(args);
            weight = args.WeightingCurve(weight);
            pool.Add(decision, weight);
        }
    }

    public void AddDecision(AIDecision decision)
    {
        decisions.Add(decision);
    }
}