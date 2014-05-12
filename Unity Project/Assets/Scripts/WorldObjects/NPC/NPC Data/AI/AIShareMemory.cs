using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIShareMemory : AIDecision
{
    public override double Cost
    {
        get { return 0; }
    }

    public override IEnumerable<AIState> States
    {
        get 
        {
            yield return AIState.Combat;
            yield return AIState.Passive;
        }
    }

    public override bool Decide(AICore core, AIDecisionCore decisionCore)
    {
        // Pulse out and share memory
        foreach (NPCMemoryItem item in core.GetNPCsUsingDecision(this))
        {
            if (item.CanSee && item.Friendly && item.AwareOf)
            {
                foreach (NPCMemoryItem selfKnowledge in core.NPCMemory.Values)
                {
                    item.NPC.AI.LearnAbout(selfKnowledge);
                }
            }
        }
        return false;
    }
}
