using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DrawAction<T>
{
    public virtual Func<T[,], bool> InitialAction { get; set; }
    public virtual Func<T[,], int, int, bool> UnitAction { get; set; }
    public virtual Func<T[,], int, int, bool> StrokeAction { get; set; }
    public virtual Func<T[,], Bounding, bool> FinalAction { get; set; }

    public DrawAction()
    {
    }

    public static implicit operator DrawAction<T>(Func<T[,], int, int, bool> normalFunc)
    {
        return new DrawAction<T>() { UnitAction = normalFunc };
    }
}
