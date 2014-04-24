using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIWander : AIRoleDecision
{
    public override AIRole Role { get { return AIRole.Other; } }
    public override double StickyShift { get { return 0d; } }
    GridSpace targetSpace;
    MultiMap<GridSpace> targetArea;

    public override double Cost { get { return 0d; } }

    public override void Action(AIActionArgs args)
    {
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
        {
            Log log = BigBoss.Debug.CreateNewLog("AI/NPC " + args.Self.ID + "/Log.txt");
            log.printHeader("AIWander Action");
            log.w("target area" + targetSpace);
            log.w("current area " + args.Self.GridSpace);
            if (targetArea != null)
            {
                log.w("Area contains? " + targetArea.Contains(args.Self.GridSpace));
            }
            log.w("At destination? " + args.Self.GridSpace.Equals(targetSpace));
            log.printFooter("AIWander Action");
            log.close();
        }
        #endregion
        if (!args.Continuing // Not continuing previous
            || !targetArea.Contains(args.Self.GridSpace.X, args.Self.GridSpace.Y) // Not in wander area
            || targetSpace == null // No target area
            || args.Self.GridSpace.Equals(targetSpace) // In target area
            )
        {
            if (targetArea == null || !targetArea.Contains(args.Self.GridSpace.X, args.Self.GridSpace.Y))
            { // Generate a new "wander area"
                targetArea = new MultiMap<GridSpace>();
                args.Level.DrawBreadthFirstFill(args.Self.GridSpace.X, args.Self.GridSpace.Y, true,
                    Draw.Walkable<GridSpace>().And(Draw.AddTo(targetArea)),
                    Draw.CountUntil<GridSpace>(25));
                targetArea[args.Self.GridSpace.X, args.Self.GridSpace.Y] = args.Self.GridSpace;
                #region DEBUG
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
                {
                    Log log = BigBoss.Debug.CreateNewLog("AI/NPC " + args.Self.ID + "/Log.txt");
                    log.printHeader("AIWander New Area");
                    targetArea.ToLog(log);
                    args.Level.ToLog(log, targetArea);
                    log.printFooter("AIWander New Area");
                    log.close();
                }
                #endregion
            }
            // Pick new targetSpace
            Value2D<GridSpace> space;
            targetArea.GetRandom(args.Random, out space);
            targetSpace = space.val;
            #region DEBUG
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.AI))
            {
                Log log = BigBoss.Debug.CreateNewLog("AI/NPC " + args.Self.ID + "/Log.txt");
                log.printHeader("AIWander Pick Target");
                log.w("New target: " + targetSpace);
                log.printFooter("AIWander Pick Target");
                log.close();
            }
            #endregion
        }
        args.MoveTo(targetSpace);
    }

    public override double CalcWeighting(AIDecisionArgs args)
    {
        if (args.Continuing)
        {
            if (targetSpace != null && targetSpace.Equals(args.Self.GridSpace))
            { // Chance to continue wandering in a different direction
                return 2d;
            }
            // Chance to continue on path
            return 20d;
        }
        // Chance to start wandering
        return 1d;
    }
}

