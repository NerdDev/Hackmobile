using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AICastDamageSpell : AIDecision
{
    int turnsSinceLastCast = 0;
    public override double Cost { get { return 60d; } }

    public AICastDamageSpell()
    {
    }

    public override void Action(AIActionArgs args)
    {
        if (args.Self.KnownSpells.ContainsKey("Fireball"))
        {
            Spell spellToCast = args.Self.KnownSpells["Fireball"];
            if (args.Self.GridDistanceToTarget(BigBoss.Player) < spellToCast.range)
            {
                args.Self.CastSpell(spellToCast, BigBoss.Player);
                turnsSinceLastCast = 0;
            }
        }
    }

    public override double CalcWeighting(AIDecisionArgs args)
    {
        if (args.Self.KnownSpells.ContainsKey("Fireball") && turnsSinceLastCast > 5)
            return 0.4d;
        else
            turnsSinceLastCast++;
        return -1.0d;
    }
}
