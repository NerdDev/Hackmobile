using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class AIAttack : AIDecision
{
    public NPC target;
    public override AIRole Role { get { return AIRole.Damage; } }

    public AIAttack()
    {
        target = BigBoss.Player; //temp
        Cost = 60;
    }

    public override void Action(AIActionArgs args)
    {
        args.NPC.attack(target);
    }

    public override double CalcWeighting(AIDecisionArgs args)
    {
        if (args.NPC.IsNextToTarget(target))
        {
            return 0.6d;
        }
        else
        {
            return -1.0d;
        }
    }
}
