using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class AIDoDamage : AIRoleDecision
{
    public override AIRole Role { get { return AIRole.Damage; } }
    public override double Cost { get { return 60d; } }
    NPC target;

    public AIDoDamage()
    {
        target = BigBoss.Player;
    }

    public override void Action(AIActionArgs args)
    {
        // Code to roll between damage options
        
        // Temporarily just doing autoattack for now.
        args.NPC.attack(target);
    }

    public override double CalcWeighting(AIDecisionArgs args)
    {
        // Do Damage is the arbitrary in-combat standard of zero
        return 0.0d;
    }
}
