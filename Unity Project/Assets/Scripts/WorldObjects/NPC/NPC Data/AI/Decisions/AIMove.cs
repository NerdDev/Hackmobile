using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIMove : AIDecision
{
    public NPC target;
    public override AIRole Role { get { return AIRole.Other; } }

    public AIMove()
    {
        target = BigBoss.Player;
        Cost = 60;
    }

    public override void Action(AIActionArgs args)
    {
        PathNode[] nodes = PathTree.Instance.getPath(args.NPC.GridSpace, target.GridSpace, 75).ToArray();
        if (nodes.Length > 2)
        {
            GridSpace nodeToMove = nodes[nodes.Length - 2].loc;
            if (nodeToMove.Type == GridType.Door && DoorCheck(nodeToMove))
            {
                return;
            }
            //else, continue on and move the NPC... if it's not a door, or if it's an open door
            args.NPC.MoveNPC(nodeToMove);
        }
    }

    public override double CalcWeighting(AIDecisionArgs args)
    {
        if (args.NPC.IsNextToTarget(target))
        {
            return -1.0d;
        }
        else
        {
            return 0.1d;
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
}
