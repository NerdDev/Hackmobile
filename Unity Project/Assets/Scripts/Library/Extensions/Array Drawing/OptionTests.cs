using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class OptionTests<T> : StrokedAction<T>
{
    public virtual Func<T[,], bool> InitialTest { get; set; }
    public virtual Func<T[,], Bounding, bool> FinalTest { get; set; }

    public static implicit operator OptionTests<T>(DrawAction<T> action)
    {
        return new OptionTests<T>() { UnitAction = action };
    }
}
