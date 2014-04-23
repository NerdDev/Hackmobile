using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIAggro : AIRoleDecision
{
    public override AIRole Role { get { return AIRole.Other; } }
    public override double Cost { get { return 0d; } }
    public override double StickyShift { get { return 0d; } }

    public override void Action(AIActionArgs args)
    {
        args.CurrentState = AIState.Combat;
    }

    public override double CalcWeighting(AIDecisionArgs args)
    {
        var player = BigBoss.Player;
        if (!Physics.Linecast(args.Self.EyeSightPosition, player.EyeSightPosition)
            && args.Random.Percent(0.6d))
        { // Can see player
            return 1000;
        }
        return 0d;

        /*
         * Need to check for other enemy NPCs.
         * We will want to aggro enemy factions only when the player can see one of the members (so they dont all kill each other when the player is across the map)
         * 
         * We may want a small chance they fight even if the player is around (leaving dead bodies)... but this should be a check that happens once and is blocked 
         * afterwards, rather than giving it a chance every frame.
         */
    }
}
