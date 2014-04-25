using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class AIDecision : IXmlParsable
{
    public abstract double Cost { get; }

    public abstract double StickyShift { get; }

    public abstract void Action(AICore core);

    public abstract double CalcWeighting(AICore core);

    public virtual void ParseXML(XMLNode x)
    {
    }
}
