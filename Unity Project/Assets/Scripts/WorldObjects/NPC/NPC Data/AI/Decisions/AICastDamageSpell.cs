using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AICastDamageSpell : AIDecision
{
    int turnsSinceLastCast = 0;
    public override AIRole Role { get { return AIRole.Damage; } }

    public AICastDamageSpell()
    {
        Cost = 60;
    }

    public override void Action(AIActionArgs args)
    {
        if (args.NPC.KnownSpells.ContainsKey("Fireball"))
        {
            Spell spellToCast = args.NPC.KnownSpells["Fireball"];
            if (args.NPC.GridDistanceToTarget(BigBoss.Player) < spellToCast.range)
            {
                args.NPC.CastSpell(spellToCast, BigBoss.Player);
                turnsSinceLastCast = 0;
            }
        }
    }

    public override double CalcWeighting(AIDecisionArgs args)
    {
        if (args.NPC.KnownSpells.ContainsKey("Fireball") && turnsSinceLastCast > 5)
            return 0.4d;
        else
            turnsSinceLastCast++;
        return -1.0d;
    }
}
