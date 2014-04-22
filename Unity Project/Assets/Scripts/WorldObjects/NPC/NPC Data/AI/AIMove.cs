using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIMove : AIAction
{
    public NPC target;

    public AIMove(NPC n)
        : base(n)
    {
        target = BigBoss.Player;
        Cost = 60;
    }

    public override void Action()
    {
        target = BigBoss.Player;
        npc.MoveNPC(target.GO.transform.position);
    }

    public override void CalcWeighting()
    {
        if (npc.IsNextToTarget(target))
        {
            Weight = 0;
        }
        else
        {
            Weight = 50;
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
