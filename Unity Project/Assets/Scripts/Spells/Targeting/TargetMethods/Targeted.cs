using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Targeted : TargetMethod
{
    public override List<WorldObject> getTargets(int x, int y)
    {
        return new List<WorldObject>();
    }
}