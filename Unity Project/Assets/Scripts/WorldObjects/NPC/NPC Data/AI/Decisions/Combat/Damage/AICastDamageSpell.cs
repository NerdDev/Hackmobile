﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AICastDamageSpell : AIDecision
{
    int turnsSinceLastCast = 0;
    public override double Cost { get { return 60d; } }
    public override double StickyShift { get { return 0d; } }

    public AICastDamageSpell()
    {
    }

    public override void Action(AICore core)
    {
        if (core.Self.KnownSpells.ContainsKey("Fireball"))
        {
            Spell spellToCast = core.Self.KnownSpells["Fireball"];
            if (core.Self.GridDistanceToTarget(BigBoss.Player) < spellToCast.range)
            {
                core.Self.CastSpell(spellToCast, BigBoss.Player);
                turnsSinceLastCast = 0;
            }
        }
    }

    public override double CalcWeighting(AICore core)
    {
        if (core.Self.KnownSpells.ContainsKey("Fireball") && turnsSinceLastCast > 5)
            return 0.4d;
        else
            turnsSinceLastCast++;
        return -1.0d;
    }
}
