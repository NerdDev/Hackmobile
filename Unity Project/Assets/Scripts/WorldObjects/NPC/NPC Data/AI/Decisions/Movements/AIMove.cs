using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIMove : AIDecision
{
    public override double Cost { get { return 60d; } }
    public override double StickyShift { get { return 2d; } }

    public AIMove()
    {
    }

    public override void Action(AICore core)
    {
        if (args.Target != null)
        {
            args.Self.MoveNPC(args.Target.GridSpace);
        }
        else
        {
            args.Self.MoveNPC(args.TargetSpace);
        }
    }

    private bool DoorCheck(GridSpace grid)
    {
        List<GameObject> blocks = grid.Blocks;
        if (blocks != null)
        {
            GameObject doorBlock = blocks.FirstOrDefault(go => go.GetComponentInChildren<Door>() != null);
            if (doorBlock != null)
            {
                Door door = doorBlock.GetComponentInChildren<Door>();
                if (door != null && !door.open)
                {
                    door.OpenDoor();
                    return true; //no moving here!
                }
            }
        }
        return false;
    }

    public override double CalcWeighting(AICore core)
    {
        return 1d;
    }
}
