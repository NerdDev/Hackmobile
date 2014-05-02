using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIPassiveHording : AIHording
{
    public float ChaseTippingRatio;

    public override bool Decide(AICore core, out double weight, out DecisionActions actions)
    {
        if (core.CurrentState == AIState.Passive)
        {
            actions = (coreP) => core.CurrentState = AIState.Combat;
            weight = 0d;

            if (ratio > ChaseTippingRatio)
            {
                return true;
            }
            return false;
        }
        else
        {
            return base.Decide(core, out weight, out actions);
        }
    }

    public override void ParseXML(XMLNode x)
    {
        base.ParseXML(x);
        ChaseTippingRatio = x.SelectFloat("ChaseTippingRatio", 2.1f);
    }
}

