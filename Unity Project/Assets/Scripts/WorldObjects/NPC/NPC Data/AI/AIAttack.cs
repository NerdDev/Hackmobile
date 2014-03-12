using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class AIAttack : AIAction
{
    public NPC target;

    public AIAttack(NPC n)
        : base(n)
    {
        target = BigBoss.Player; //temp
        Cost = 60;
    }

    public override void Action()
    {
        npc.attack(target);
    }

    public override void CalcWeighting()
    {
        if (npc.IsNextToTarget(target))
        {
            Weight = 50;
        }
        else
        {
            Weight = 0;
        }
    }
}
