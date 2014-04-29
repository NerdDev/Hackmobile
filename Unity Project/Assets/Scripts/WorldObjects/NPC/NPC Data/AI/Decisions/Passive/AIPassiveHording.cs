using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIPassiveHording : AIAggro
{
    public float TippingRatio;
    public float FleeDistance;
    private float ratio;
    private NPC closestEnemy;

    public override void Action(AICore core)
    {
        if (ratio > TippingRatio)
        {
            core.CurrentState = AIState.Combat;
            return;
        }

        core.MoveAway(core.Target);
    }

    public override bool CalcWeighting(AICore core, out double weight)
    {
        weight = 0d;
        ratio = core.NumFriendlies;
        ratio /= core.NumEnemies;
        if (ratio > TippingRatio)
        {
            return true;
        }

        if (core.ClosestEnemyDist < FleeDistance)
        {
            core.Target = core.ClosestEnemy;
            return true;
        }
        
        return false;
    }

    public override void ParseXML(XMLNode x)
    {
        base.ParseXML(x);
        TippingRatio = x.SelectFloat("TippingRatio", 2.1f);
        FleeDistance = x.SelectFloat("FleeDistance", 7f);

    }
}

