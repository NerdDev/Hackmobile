using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class OptionTests<T> : DrawActions<T>
{
    public virtual Func<T[,], bool> InitialTest { get; set; }
    public virtual Func<T[,], Bounding, bool> FinalTest { get; set; }
}
