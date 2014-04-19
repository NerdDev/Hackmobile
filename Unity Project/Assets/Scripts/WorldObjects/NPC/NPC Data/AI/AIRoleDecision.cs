using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class AIRoleDecision : AIDecision
{
    public abstract AIRole Role { get; }
}
