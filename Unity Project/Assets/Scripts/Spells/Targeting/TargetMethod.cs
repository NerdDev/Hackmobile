using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TargetMethod
{
    public virtual List<IAffectable> getTargets()
    {
        return new List<IAffectable>();
    }

    public virtual List<IAffectable> getTargets(int x, int y)
    {
        return new List<IAffectable>();
    }

    public virtual WorldObject getSource()
    {
        throw new NotImplementedException();
    }

    public virtual bool takesInput()
    {
        return false;
    }

    public virtual void inputLocation(int x, int y)
    {
    }
}
