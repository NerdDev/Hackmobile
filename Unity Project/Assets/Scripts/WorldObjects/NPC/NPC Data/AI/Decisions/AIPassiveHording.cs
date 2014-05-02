using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIPassiveHording : AIHording
{
    public float ChaseTippingRatio;

    public override IEnumerable<AIState> States
    {
        get
        {
            yield return AIState.Combat;
            yield return AIState.Passive;
        }
    }


    public override bool Decide(AICore core, out double weight, out DecisionActions actions)
    {
        bool hordingRet = base.Decide(core, out weight, out actions);
        if (core.CurrentState == AIState.Passive)
        {
            if (ratio > ChaseTippingRatio)
            {
                actions = (coreP) => core.CurrentState = AIState.Combat;
                weight = 0d;
                return true;
            }
        }
        return hordingRet;
    }

    public override void ParseXML(XMLNode x)
    {
        base.ParseXML(x);
        ChaseTippingRatio = x.SelectFloat("ChaseTippingRatio", 2.1f);
    }
}

