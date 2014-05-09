using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIHording : AIDecision, ICopyable
{
    public override double Cost { get { return 0; } }

    public override IEnumerable<AIState> States
    {
        get
        {
            yield return AIState.Combat;
        }
    }

    public float AttackTippingRatio;
    public float CirclingBuffer;
    protected float ratio;
    private const double circleChangeChance = .2d;
    private bool clockwise;
    AIFlee fleePackage = new AIFlee();

    public void PostPrimitiveCopy()
    {
        clockwise = Probability.Rand.NextBool();
    }

    public void PostObjectCopy()
    {
    }

    public override bool Decide(AICore core, AIDecisionCore decisionCore)
    {
        Args.Weight = 0d;
        if (core.NumEnemies == 0)
        {
            Args.Actions = null;
            return false;
        }

        ratio = core.NumFriendlies + 1;
        ratio /= core.NumEnemies;
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            core.Log.w("Ratio " + ratio);
        }
        #endregion
        if (ratio >= AttackTippingRatio)
        { // Release to other AI
            #region DEBUG
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
            {
                core.Log.w("Releasing to AI");
            }
            #endregion
            Args.Actions = null;
            return false;
        }

        bool instantPick;
        if (PassControl(core, decisionCore, fleePackage, out instantPick))
        {
            return instantPick;
        }

        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            core.Log.w("Closest enemy dist: " + core.ClosestEnemyDist + "  Flee Dist: " + fleePackage.FleeTriggerDistance + "  Circling Buffer: " + CirclingBuffer);
        }
        #endregion
        if (core.ClosestEnemyDist < fleePackage.FleeTriggerDistance + CirclingBuffer)
        { // Close enough to NPC, so substitute movement to circle to run around him
            GridSpace circleSpace;
            if (core.Random.Percent(circleChangeChance))
            { // Changing circling directions
                clockwise = !clockwise;
            }
            float angle = Vector3.Angle(core.Self.GO.transform.position, core.ClosestEnemy.GO.transform.position);
            circleSpace = core.Level.GetFromTangent(core.Self.GridSpace.X, core.Self.GridSpace.Y, angle, clockwise);
            if (!circleSpace.Walkable())
            {
                circleSpace = core.Self.GridSpace;
            }
            core.MovementSubstitutions[core.ClosestEnemy.GridSpace] = circleSpace.Walkable() ? circleSpace : core.Self.GridSpace;
            #region DEBUG
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
            {
                core.Log.w("Circling.  Angle: " + angle + " Sub Space: " + circleSpace);
            }
            #endregion
        }
        // Release to other AI
        Args.Actions = null;
        return false;
    }

    public override void ParseXML(XMLNode x)
    {
        base.ParseXML(x);
        AttackTippingRatio = x.SelectFloat("AttackTippingRatio", 4.1f);
        CirclingBuffer = x.SelectFloat("CirclingBuffer", 1f);
        fleePackage.ParseXML(x);
    }
}

