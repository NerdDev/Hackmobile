using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IGridSpace
{
    GridType Type { get; set; }
    Theme Theme { get; set; }
    int X { get; }
    int Y { get; }
}

