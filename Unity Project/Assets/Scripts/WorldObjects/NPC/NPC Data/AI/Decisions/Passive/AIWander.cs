using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIWander : AIRoleDecision
{
    public override AIRole Role { get { return AIRole.Other; } }
    public override double StickyShift { get { return 10d; } }
    GridSpace targetSpace;

    public override double Cost { get { return 0d; } }

    public override void Action(AIActionArgs args)
    {
        if (!args.Continuing)
        { // Pick new targetSpace
            //args.Level.DrawBreadthFirstFill(args.Self.GridSpace.X, args.Self.GridSpace.Y, true, Draw.Walkable(), Draw.CountUntil<GridSpace>(200));
        }
        args.MoveTo(targetSpace);
    }

    public override double CalcWeighting(AIDecisionArgs args)
    {
        if (args.Continuing && targetSpace.Equals(args.Self.GridSpace))
        {
            return 0.0d;
        }
        return 0.05d;
    }
}

