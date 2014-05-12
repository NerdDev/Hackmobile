using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AICastDamageSpell : AIDecision
{
    int turnsSinceLastCast = 0;
    public override double Cost { get { return 60d; } }
    public override IEnumerable<AIState> States { get { yield return AIState.Combat; } }

    public AICastDamageSpell()
    {
    }

    public override bool Decide(AICore core, AIDecisionCore decisionCore)
    {
        if (core.Self.KnownSpells.ContainsKey("Fireball") && turnsSinceLastCast > 5)
        {
            Args.Weight = 0.4d;
            Args.Actions = (coreP) =>
            {
                Spell spellToCast = core.Self.KnownSpells["Fireball"];
                if (core.Self.DistanceToTarget(BigBoss.Player) < spellToCast.range)
                {
                    core.Self.CastSpell(spellToCast, BigBoss.Player);
                    turnsSinceLastCast = 0;
                }
            };
            return false;
        }
        else
            turnsSinceLastCast++;
        Args.Weight = -1.0d;
        Args.Actions = null;
        return true;
    }
}
