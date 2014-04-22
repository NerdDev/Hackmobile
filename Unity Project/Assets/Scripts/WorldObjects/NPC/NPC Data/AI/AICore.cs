using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AICore
{
    internal List<AIAction> actions = new List<AIAction>();
    NPC npc;
    public Role role;

    //initialization parameters
    public AICore()
    {
    }
    public AICore(NPC n)
    {
        InitAI(n);
    }
    public AICore(NPC n, params AIAction[] aiParams)
    {
        InitAI(n, aiParams);
    }
    public AICore(NPC n, List<AIAction> aiParams)
    {
        InitAI(n, aiParams);
    }
    public AICore(NPC n, Role role)
    {
        InitAI(n, role);
    }
    public void InitAI(NPC n, params AIAction[] aiParams)
    {
        this.npc = n;
        actions.AddRange(aiParams);
    }
    public void InitAI(NPC n, List<AIAction> aiParams)
    {
        this.npc = n;
        actions.AddRange(aiParams);
    }
    public void InitAI(NPC n, Role role)
    {
        this.npc = n;
        //if the NPC has a role, we've got a default AI setup
        switch (role)
        {
            case Role.MAGE:
                actions.Add(new AIAttack(npc));
                actions.Add(new AICastDamageSpell(npc));
                actions.Add(new AIMove(npc));

                break;
            default:
                InitAI(n);
                break;
        }
    }

    public void InitAI(NPC n)
    {
        this.npc = n;
        actions.Add(new AIAttack(n));
        actions.Add(new AIMove(n));
    }

    public void DecideWhatToDo()
    {
        foreach (AIAction action in actions)
        {
            action.CalcWeighting();
        }
        actions.Sort();
        if (actions[actions.Count - 1].Weight > 0)
        {
            actions[actions.Count - 1].Action();
        }
    }
}
