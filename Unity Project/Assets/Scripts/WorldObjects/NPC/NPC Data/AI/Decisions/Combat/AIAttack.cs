using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class AIAttack : AIRoleDecision
{
    public override AIRole Role { get { return AIRole.Damage; } }
    public override double Cost { get { return 60d; } }
    NPC target;

    public AIAttack()
    {
        target = BigBoss.Player;
    }

    public override void Action(AIActionArgs args)
    {
        var player = BigBoss.Player;
        args.NPC.attack(target);
    }

    public override double CalcWeighting(AIDecisionArgs args)
    {
        var player = BigBoss.Player;
        if (args.NPC.IsNextToTarget(player))
        {
            return 0.6d;
        }
        else
        {
            return -1.0d;
        }
    }
}
