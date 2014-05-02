using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AICastDamageSpell : AIDecision
{
    int turnsSinceLastCast = 0;
    public override double Cost { get { return 60d; } }
    public override double StickyShift { get { return 0d; } }
    public override IEnumerable<AIState> States { get { yield return AIState.Combat; } }

    public AICastDamageSpell()
    {
    }

    public override bool Decide(AICore core, out double weight, out DecisionActions actions)
    {
        if (core.Self.KnownSpells.ContainsKey("Fireball") && turnsSinceLastCast > 5)
        {
            weight = 0.4d;
            actions = (coreP) =>
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
        weight = -1.0d;
        actions = null;
        return false;
    }
}
