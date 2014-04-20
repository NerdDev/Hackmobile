using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class AIAction
{
    public abstract double Cost { get; }

    public abstract void Action(AIActionArgs args);
}
