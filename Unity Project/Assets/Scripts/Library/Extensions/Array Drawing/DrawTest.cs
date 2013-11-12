using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DrawTest<T>
{
    public Func<T[,], bool> InitialTest { get; set; }
    public Func<T[,], int, int, bool> UnitTest { get; set; }

    public static implicit operator DrawTest<T>(Func<T[,], int, int, bool> normalFunc)
    {
        return new DrawTest<T>() { UnitTest = normalFunc };
    }
}
