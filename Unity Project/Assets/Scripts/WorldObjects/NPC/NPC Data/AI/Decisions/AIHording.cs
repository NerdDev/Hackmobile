using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIHording : AIDecision, ICopyable
{
    public override double Cost { get { return 0; } }

    public override double StickyShift { get { return 0; } }

    public override IEnumerable<AIState> States
    {
        get
        {
            yield return AIState.Combat;
        }
    }

    public float AttackTippingRatio;
    public float FleeDistance;
    public float FleeWeight;
    public float CirclingBuffer;
    protected float ratio;
    private const double circleChangeChance = .2d;
    bool clockwise;

    public void PostPrimitiveCopy()
    {
        clockwise = Probability.Rand.NextBool();
    }

    public void PostObjectCopy()
    {
    }

    public override bool Decide(AICore core, out double weight, out DecisionActions actions)
    {
        weight = 0d;
        if (core.NumEnemies == 0)
        {
            actions = null;
            return false;
        }

        ratio = core.NumFriendlies;
        ratio /= core.NumEnemies;
        if (ratio > AttackTippingRatio)
        { // Release to other AI
            actions = null;
            return false;
        }

        if (core.ClosestEnemyDist < FleeDistance)
        { // Run
            weight = FleeWeight;
            actions = (coreP) =>
            {
                coreP.MoveAway(core.ClosestEnemy);
            };
            return false;
        }
        else if (core.ClosestEnemyDist < FleeDistance + CirclingBuffer)
        { // Close enough to NPC, so substitute movement to circle to run around him
            GridSpace circleSpace;
            if (core.Random.Percent(circleChangeChance))
            { // Changing circling directions
                clockwise = !clockwise;
            }
            float angle = Vector3.Angle(core.Self.GO.transform.position, core.ClosestEnemy.GO.transform.position);
            circleSpace = core.Level.GetFromTangent(core.Self.GridSpace.X, core.Self.GridSpace.Y, angle, clockwise);
            core.MovementSubstitutions[core.ClosestEnemy.GridSpace] = circleSpace.Walkable() ? circleSpace : core.Self.GridSpace;
        }
        // Release to other AI
        actions = null;
        return false;
    }

    public override void ParseXML(XMLNode x)
    {
        base.ParseXML(x);
        AttackTippingRatio = x.SelectFloat("AttackTippingRatio", 4.1f);
        FleeDistance = x.SelectFloat("FleeDistance", 7f);
        CirclingBuffer = x.SelectFloat("CirclingBuffer", 1f);
        FleeWeight = x.SelectFloat("FleeWeight", 3f);
    }
}

