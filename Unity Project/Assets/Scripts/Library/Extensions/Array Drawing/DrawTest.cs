using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DrawTest<T>
{
    public virtual Func<T[,], bool> InitialTest { get; set; }
    public virtual Func<T[,], int, int, bool> UnitTest { get; set; }
    public virtual Func<T[,], int, int, bool> StrokeTest { get; set; }
    public virtual Func<T[,], Bounding, bool> FinalTest { get; set; }

    public DrawTest()
    {
    }

    public static implicit operator DrawTest<T>(Func<T[,], int, int, bool> normalFunc)
    {
        return new DrawTest<T>() { UnitTest = normalFunc };
    }
}
