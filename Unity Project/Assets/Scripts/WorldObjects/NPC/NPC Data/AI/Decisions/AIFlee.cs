using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIFlee : AIDecision
{
    public float FleeTriggerDistance;
    public float FleeWeight;
    public float FleeLookRange;
    public float EndingFleeWeightSkew;

    public override double Cost
    {
        get { return 0; }
    }

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
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            core.Log.w("Closest enemy dist: " + core.ClosestEnemyDist + "  Flee Dist: " + FleeTriggerDistance + " Continuing: " + decisionCore.Continuing(this) + " LastMovementGoal: " + core.LastMovementGoal);
        }
        #endregion
        if (core.ClosestEnemyDist < FleeTriggerDistance)
        { // Run
            Args.Weight = FleeWeight;
            Args.StickyReduc = 3;
            Args.Actions = (coreP) =>
            {
                coreP.FleeNPCs(x => !x.Friendly, FleeTriggerDistance, FleeLookRange);
            };
            #region DEBUG
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
            {
                core.Log.w("Running away");
            }
            #endregion
            return true;
        }
        if (!decisionCore.Continuing(this)) return false;
        if (core.LastMovementGoal == null || core.Self.GridSpace.Equals(core.LastMovementGoal)) return false;

        Args.Actions = (corep) =>
        {
            core.MoveTo(core.LastMovementGoal);
        };
        Args.Weight = FleeWeight * EndingFleeWeightSkew;
        return true;
    }

    public override void ParseXML(XMLNode x)
    {
        base.ParseXML(x);
        FleeTriggerDistance = x.SelectFloat("FleeTriggerDistance", 7f);
        FleeWeight = x.SelectFloat("FleeWeight", 15f);
        FleeLookRange = x.SelectFloat("FleeLookRange", 6);
        EndingFleeWeightSkew = x.SelectFloat("EndingFleeWeightSkew", .75f);
    }
}
