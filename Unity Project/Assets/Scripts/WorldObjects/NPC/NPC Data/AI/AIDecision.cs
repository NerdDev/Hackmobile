using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class AIDecision : IXmlParsable
{
    public abstract double Cost { get; }

    public abstract double StickyShift { get; }

    public abstract void Action(AICore core);

    public abstract bool CalcWeighting(AICore core, out double weight);

    public virtual void ParseXML(XMLNode x)
    {
    }
}
