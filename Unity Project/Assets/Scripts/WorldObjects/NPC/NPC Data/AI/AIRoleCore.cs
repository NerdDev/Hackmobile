using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIRoleCore
{
    AIDecisionCore[] cores = new AIDecisionCore[EnumExt.Length<AIRole>()];

    public void FillPool(AICore core, ProbabilityPool<AIDecision> pool, AIDecisionArgs args)
    {
        for (int i = 0; i < cores.Length; i++)
        {
            AIDecisionCore decisionCore = cores[i];
            if (decisionCore == null) continue;
            double roleWeight = core.RoleWeights[i];
            args.WeightingCurve = (weight) =>
            {
                return weight + roleWeight;
            };
            decisionCore.FillPool(pool, args);
        }
    }

    public void AddDecision(AIRoleDecision decision)
    {
        AIDecisionCore decisionCore = cores[(int)decision.Role];
        if (decisionCore == null)
        {
            decisionCore = new AIDecisionCore();
            cores[(int)decision.Role] = decisionCore;
        }
        decisionCore.AddDecision(decision);
    }
}
