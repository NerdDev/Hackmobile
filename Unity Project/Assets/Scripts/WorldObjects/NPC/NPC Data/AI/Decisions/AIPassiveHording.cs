using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIPassiveHording : AIHording
{
    public float ChaseTippingRatio;

    public override void Action(AICore core)
    {
        if (core.CurrentState == AIState.Passive)
        {
            if (ratio > ChaseTippingRatio)
            {
                core.CurrentState = AIState.Combat;
                return;
            }
        }
        else
        {
            base.Action(core);
        }
    }

    public override bool CalcWeighting(AICore core, out double weight)
    {
        if (core.CurrentState == AIState.Passive)
        {
            weight = 0d;
            if (ratio > ChaseTippingRatio)
            {
                return true;
            }
            return false;
        }
        else
        {
            return base.CalcWeighting(core, out weight);
        }
    }

    public override void ParseXML(XMLNode x)
    {
        base.ParseXML(x);
        ChaseTippingRatio = x.SelectFloat("ChaseTippingRatio", 2.1f);
    }
}

