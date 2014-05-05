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


    public override bool Decide(AICore core)
    {
        bool hordingRet = base.Decide(core);
        if (core.CurrentState == AIState.Passive)
        {
            if (ratio > ChaseTippingRatio)
            {
                Args.Actions = (coreP) => core.CurrentState = AIState.Combat;
                return true;
            }
            if (hordingRet)
            { // Force flee
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

