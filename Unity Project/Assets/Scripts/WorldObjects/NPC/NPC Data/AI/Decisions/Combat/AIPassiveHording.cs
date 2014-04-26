using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class AIPassiveHording : AIAggro
{
    public override double Cost { get { return 0d; } }

    public override double StickyShift { get { return 0d; } }

    public int CombatLimit;

    public override void Action(AICore core)
    {

    }

    public override bool CalcWeighting(AICore core, out double weight)
    {
        if (base.CalcWeighting(core, out weight))
        { // Should aggro player
            return true;
        }
        weight = 0d;
        return false;
    }

    public override void ParseXML(XMLNode x)
    {
        base.ParseXML(x);
        CombatLimit = x.SelectInt("CombatLimit", 3);
    }
}

