using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class AIUseAbility : AIRoleDecision
{
    public override AIRole Role { get { return AIRole.Damage; } }
    public override double Cost { get { return 60d; } }

    public AIUseAbility()
    {
    }

    public override void Action(AIActionArgs args)
    {
        // Code to roll between damage options
        
        // Temporarily just doing autoattack for now.
        if (args.Self.IsNextToTarget(args.Target))
        {
            args.Self.attack(args.Target);
        }
        else
        {
            args.MoveTo(args.Target);
        }
    }

    public override double CalcWeighting(AIDecisionArgs args)
    {
        // Do Damage is the arbitrary in-combat standard of zero
        return 0.0d;
    }
}
