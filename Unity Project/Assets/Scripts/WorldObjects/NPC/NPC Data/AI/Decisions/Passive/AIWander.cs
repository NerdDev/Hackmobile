using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIWander : AIDecision
{
    public override double StickyShift { get { return 0d; } }
    GridSpace targetSpace;
    MultiMap<GridSpace> targetArea;

    public override double Cost { get { return 0d; } }

    public override void Action(AICore core)
    {
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            Log log = BigBoss.Debug.CreateNewLog("AI/NPC " + core.Self.ID + "/Log.txt");
            log.printHeader("AIWander Action");
            log.w("target area" + targetSpace);
            log.w("current area " + core.Self.GridSpace);
            if (targetArea != null)
            {
                log.w("Area contains? " + targetArea.Contains(core.Self.GridSpace));
            }
            log.w("At destination? " + core.Self.GridSpace.Equals(targetSpace));
            log.printFooter("AIWander Action");
            log.close();
        }
        #endregion
        if (!core.Continuing(this) // Not continuing previous
            || !targetArea.Contains(core.Self.GridSpace.X, core.Self.GridSpace.Y) // Not in wander area
            || targetSpace == null // No target area
            || core.Self.GridSpace.Equals(targetSpace) // In target area
            )
        {
            if (targetArea == null || !targetArea.Contains(core.Self.GridSpace.X, core.Self.GridSpace.Y))
            { // Generate a new "wander area"
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
            // Pick new targetSpace
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
        core.MoveTo(targetSpace);
    }

    public override bool CalcWeighting(AICore core, out double weight)
    {
        if (core.Continuing(this))
        {
            if (targetSpace != null && targetSpace.Equals(core.Self.GridSpace))
            { // Chance to continue wandering in a different direction
                weight = 2d;
                return false;
            }
            // Chance to continue on path
            weight = 20d;
            return false;
        }
        // Chance to start wandering
        weight = 0.15d;
        return false;
    }
}

