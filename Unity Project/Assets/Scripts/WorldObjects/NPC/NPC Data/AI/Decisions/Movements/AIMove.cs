using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIMove : AIDecision, ICopyable
{
    public override double Cost { get { return 60d; } }
    public override IEnumerable<AIState> States { get { yield return AIState.Movement; } }

    public AIMove()
    {
    }

    public void Move(AICore core)
    {
        if (core.Target != null)
        {
            core.Self.MoveNPC(core.Target.GO.transform.position);
        }
        else
        {
            core.Self.MoveNPC(core.TargetSpace);
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

    public override bool Decide(AICore core)
    {
        return false;
    }

    public void PostPrimitiveCopy()
    {
    }

    public void PostObjectCopy()
    {
        Args.Weight = 1d;
        Args.Actions = Move;
    }
}
