using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIPassiveHording : AIDecision
{
    public override double Cost { get { return 0d; } }

    public override double StickyShift { get { return 0d; } }

    public int CombatLimit;

    static AIAggro AggroPackage = new AIAggro();

    public override void Action(AICore core)
    {

    }

    public override double CalcWeighting(AICore core)
    {
        if (AggroPackage.CalcWeighting(core) > 0)
        { // Should aggro player

        }
        return 0d;
    }

    public override void ParseXML(XMLNode x)
    {
        base.ParseXML(x);
        CombatLimit = x.SelectInt("CombatLimit", 3);
    }
}

