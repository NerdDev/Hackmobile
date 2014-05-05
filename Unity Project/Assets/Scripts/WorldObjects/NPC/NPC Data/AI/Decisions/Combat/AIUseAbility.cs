using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class AIUseAbility : AIDecision,  ICopyable
{
    public override double Cost { get { return 60d; } }
    public override IEnumerable<AIState> States { get { yield return AIState.Combat; } }

    public AIUseAbility()
    {
    }

    public void PostPrimitiveCopy()
    {

    }

    public void PostObjectCopy()
    {
        // Do Damage is the arbitrary in-combat standard of 1
        Args.Weight = 1d;
        Args.Actions = UseAbility;
    }

    protected void UseAbility(AICore core)
    {
        // Code to roll between damage options

        // Temporarily just doing autoattack for now.
        NPC n = core.Target as NPC;
        n = BigBoss.Player; // Temp
        if (n == null)
        {
            UnityEngine.Debug.LogError("Cannot attack non-npc " + core.Target);
        }
        if (core.Self.IsNextToTarget(n))
        {
            core.Self.attack(n);
        }
        else
        {
            core.MoveTo(n);
        }
    }

    public override bool Decide(AICore core, AIDecisionCore decisionCore)
    {
        return false;
    }
}
