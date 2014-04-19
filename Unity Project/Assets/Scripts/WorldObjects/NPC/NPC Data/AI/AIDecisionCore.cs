using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIDecisionCore
{
    List<AIRoleDecision> decisions = new List<AIRoleDecision>();

    public void FillPool(ProbabilityPool<AIRoleDecision> pool, AIDecisionArgs args)
    {
        foreach (AIRoleDecision decision in decisions)
        {
            double weight = decision.CalcWeighting(args);
            weight = args.WeightingCurve(weight);
            pool.Add(decision, weight);
        }
    }

    public void AddDecision(AIRoleDecision decision)
    {
        decisions.Add(decision);
    }
}