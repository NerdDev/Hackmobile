using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class AIUseAbility : AIDecision
{
    public override double Cost { get { return 60d; } }
    public override double StickyShift { get { return 0d; } }
    public override IEnumerable<AIState> States { get { yield return AIState.Combat; } }

    public AIUseAbility()
    {
    }

    public override void Action(AICore core)
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

    public override bool CalcWeighting(AICore core, out double weight)
    {
        // Do Damage is the arbitrary in-combat standard of 1
        weight = 1d;
        return false;
    }
}
