using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        PathNode[] nodes = PathTree.Instance.getPath(npc.GridSpace, target.GridSpace, 75).ToArray();
        if (nodes.Length > 2)
        {
            GridSpace nodeToMove = nodes[nodes.Length - 2].loc;
            npc.MoveNPC(nodeToMove);
        }
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
}
