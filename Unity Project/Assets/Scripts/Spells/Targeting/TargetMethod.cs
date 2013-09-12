using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TargetMethod
{
    public virtual List<WorldObject> getTargets()
    {
        return new List<WorldObject>();
    }

    public virtual List<WorldObject> getTargets(int x, int y)
    {
        return new List<WorldObject>();
    }

    public virtual NPC getSource()
    {
        return new NPC();
    }

    public virtual bool takesInput()
    {
        return false;
    }

    public virtual void inputLocation(int x, int y)
    {
    }
}