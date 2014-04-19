using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIActionArgs
{
    private AICore core;
    public NPC NPC { get { return core.NPC; } }
    public AIState CurrentState { get { return core.CurrentState; } set { core.CurrentState = value; } }

    public AIActionArgs(AICore core)
    {
        this.core = core;
    }

    public void MoveTo(float x, float y)
    {

    }
}
