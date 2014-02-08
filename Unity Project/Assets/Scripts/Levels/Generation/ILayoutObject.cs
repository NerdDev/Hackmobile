using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ILayoutObject
{
    bool ContainsPoint(Point pt);
    void Shift(int x, int y);
    LayoutObject Bake();
    void ToLog(Logs log, params String[] customContent);
}