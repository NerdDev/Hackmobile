using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StrokedAction<T>
{
    public DrawAction<T> UnitAction;
    public DrawAction<T> StrokeAction;

    public StrokedAction()
    {
    }

    public static implicit operator StrokedAction<T>(DrawAction<T> normalFunc)
    {
        if (normalFunc == null) return null;
        return new StrokedAction<T>() { UnitAction = normalFunc };
    }

    public static implicit operator DrawAction<T>(StrokedAction<T> actions)
    {
        return actions.UnitAction;
    }

    public StrokedAction<T> Then(StrokedAction<T> rhs)
    {
        if (rhs == null) return rhs;
        return new StrokedAction<T>()
        {
            UnitAction = ((DrawAction<T>)UnitAction).And(rhs.UnitAction),
            StrokeAction = ((DrawAction<T>)StrokeAction).And(rhs.StrokeAction)
        };
    }
}
