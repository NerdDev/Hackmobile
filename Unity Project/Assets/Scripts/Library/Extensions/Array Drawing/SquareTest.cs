using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SquareTest<T> : DrawTest<T>
{
    public Func<T[,], int, int, bool> StrokeTest { get; set; }
    public Func<T[,], Bounding, bool> FinalTest { get; set; }
}
