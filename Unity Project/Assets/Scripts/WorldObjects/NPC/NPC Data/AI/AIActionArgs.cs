using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIActionArgs
{
    private AICore core;
    public NPC NPC { get { return core.NPC; } }

    public AIActionArgs(AICore core)
    {
        this.core = core;
    }
}
