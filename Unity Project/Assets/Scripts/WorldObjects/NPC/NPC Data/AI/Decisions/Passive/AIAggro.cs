using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIAggro : AIRoleDecision
{
    public override AIRole Role { get { return AIRole.Other; } }
    public override double Cost { get { return 0d; } }

    public override void Action(AIActionArgs args)
    {
        args.CurrentState = AIState.Combat;
    }

    public override double CalcWeighting(AIDecisionArgs args)
    {
        var player = BigBoss.Player;
        if (Physics.Linecast(args.Self.EyeSightPosition, player.EyeSightPosition))
        {
            return -1d;
        }
        else
        { // Can see player
            return 1d;
        }
    }
}
