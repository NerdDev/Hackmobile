using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DrawTest<T>
{
    public Func<T[,], bool> InitialTest { get; set; }
    public Func<T[,], int, int, bool> UnitTest { get; set; }
    public Func<T[,], int, int, bool> StrokeTest { get; set; }
    public Func<T[,], bool> FinalTest { get; set; }

    public static implicit operator DrawTest<T>(Func<T[,], int, int, bool> squareTest)
    {
        return new DrawTest<T>() { UnitTest = squareTest };
    }
}
