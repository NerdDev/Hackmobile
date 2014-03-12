using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class AIAction : IComparable<AIAction>
{
    private AIAction() { }
    public AIAction(NPC n)
    {
        this.npc = n;
    }
    internal NPC npc;
    public int Cost { get; set; }
    internal int Weight { get; set; }
    public abstract void Action();
    public abstract void CalcWeighting();
    public int CompareTo(AIAction other)
    {
        if (this.Weight < other.Weight)
        {
            return -1;
        }
        else if (this.Weight == other.Weight)
        {
            return 0;
        }
        return 1;
    }
}
