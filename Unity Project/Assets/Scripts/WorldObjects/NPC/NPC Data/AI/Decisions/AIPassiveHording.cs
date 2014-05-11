﻿using System;
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


    public override bool Decide(AICore core, AIDecisionCore decisionCore)
    {
        bool hordingRet = base.Decide(core, decisionCore);
        if (core.CurrentState == AIState.Passive)
        {
            if (ratio >= ChaseTippingRatio)
            {
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
                {
                    core.Log.w("Switching to combat");
                }
                Args.Actions = (coreP) => core.CurrentState = AIState.Combat;
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