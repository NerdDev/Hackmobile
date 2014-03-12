using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AICastDamageSpell : AIAction
{
    int turnsSinceLastCast = 0;
    Spell spell;

    public AICastDamageSpell(NPC n)
        : base(n)
    {
        Cost = 60;
    }

    public override void Action()
    {
        if (npc.KnownSpells.ContainsKey("Fireball"))
        {
            Spell spellToCast = npc.KnownSpells["Fireball"];
            if (npc.GridDistanceToTarget(BigBoss.Player) < spellToCast.range)
            {
                npc.CastSpell(spellToCast, BigBoss.Player);
                turnsSinceLastCast = 0;
            }
        }
    }

    public override void CalcWeighting()
    {
        if (npc.KnownSpells.ContainsKey("Fireball") && turnsSinceLastCast > 5)
            Weight = 60;
        else
            turnsSinceLastCast++;
        Weight = 0;
    }
}
