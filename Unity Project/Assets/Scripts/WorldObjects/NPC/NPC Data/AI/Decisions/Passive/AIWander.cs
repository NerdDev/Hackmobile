using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIWander : AIDecision
{
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
            core.Log.printHeader("AIWander New Area");
            targetArea.ToLog(core.Log);
            core.Level.ToLog(core.Log, targetArea);
            core.Log.printFooter("AIWander New Area");
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
            core.Log.printHeader("AIWander Pick Target");
            core.Log.w("New target: " + targetSpace);
            core.Log.printFooter("AIWander Pick Target");
        }
        #endregion
    }

    public override bool Decide(AICore core, AIDecisionCore decisionCore)
    {
        Args.Actions = AIDecisions.Base();
        if (!targetArea.Contains(core.Self.GridSpace.X, core.Self.GridSpace.Y))
        {
            Args.Actions = Args.Actions.Then(RegenAreaAction);
            targetSpace = core.Self.GridSpace;
        }
        if (decisionCore.Continuing(this) && core.Self.GridSpace.Equals(targetSpace)) // At end goal
        { // Chance to continue wandering in a different direction
            Args.Actions = Args.Actions.Then(PickNewTarget);
            Args.Weight = 2d;
        }
        else
        {
            // Chance to start wandering
            Args.Actions = Args.Actions.Then(PickNewTarget);
            Args.Weight = .15d;
            Args.StickyShift = 4.85d;
            Args.StickyReduc = 5d;
        }
        Args.Actions = Args.Actions.Then((coreP) => core.MoveTo(targetSpace));
        return false;
    }
}

