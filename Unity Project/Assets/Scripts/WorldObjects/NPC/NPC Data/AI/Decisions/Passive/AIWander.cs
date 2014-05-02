using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIWander : AIDecision
{
    public override double StickyShift { get { return 0d; } }
    public override IEnumerable<AIState> States { get { yield return AIState.Passive; } } 
    GridSpace targetSpace;
    MultiMap<GridSpace> targetArea = new MultiMap<GridSpace>();

    public override double Cost { get { return 0d; } }

    protected void RegenAreaAction(AICore core)
    {
        targetArea = new MultiMap<GridSpace>();
        core.Level.DrawBreadthFirstFill(core.Self.GridSpace.X, core.Self.GridSpace.Y, true,
            Draw.Walkable<GridSpace>().And(Draw.AddTo(targetArea)),
            Draw.CountUntil<GridSpace>(25));
        targetArea[core.Self.GridSpace.X, core.Self.GridSpace.Y] = core.Self.GridSpace;
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            Log log = BigBoss.Debug.CreateNewLog("AI/NPC " + core.Self.ID + "/Log.txt");
            log.printHeader("AIWander New Area");
            targetArea.ToLog(log);
            core.Level.ToLog(log, targetArea);
            log.printFooter("AIWander New Area");
            log.close();
        }
        #endregion
    }

    protected void PickNewTarget(AICore core)
    {
        Value2D<GridSpace> space;
        targetArea.GetRandom(core.Random, out space);
        targetSpace = space.val;
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            Log log = BigBoss.Debug.CreateNewLog("AI/NPC " + core.Self.ID + "/Log.txt");
            log.printHeader("AIWander Pick Target");
            log.w("New target: " + targetSpace);
            log.printFooter("AIWander Pick Target");
            log.close();
        }
        #endregion
    }

    public override bool Decide(AICore core, out double weight, out DecisionActions actions)
    {
        actions = AIDecisions.Base();
        if (!targetArea.Contains(core.Self.GridSpace.X, core.Self.GridSpace.Y))
        {
            actions.Then(RegenAreaAction);
            targetSpace = core.Self.GridSpace;
        }
        if (core.Continuing(this))
        {
            if (core.Self.GridSpace.Equals(targetSpace)) // At end goal
            { // Chance to continue wandering in a different direction
                actions.Then(PickNewTarget);
                weight = 2d;
            }
            else
            {
                // Chance to continue on path
                weight = 20d;
            }
        }
        else
        {
            // Chance to start wandering
            actions.Then(PickNewTarget);
            weight = 0.15d;
        }
        actions.Then((coreP) => core.MoveTo(targetSpace));
        return false;
    }
}

