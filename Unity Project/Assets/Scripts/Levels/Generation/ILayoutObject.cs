using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ILayoutObject
{
    Bounding Bounding { get; }
    bool ContainsPoint(Point pt);
    void Shift(int x, int y);
    Container2D<GridType> GetGrid();
    void ToLog(Logs log, params String[] customContent);
}

public static class ILayoutObjectExt
{
    public static void CenterOn(this ILayoutObject obj1, ILayoutObject rhs)
    {
        Point center = obj1.Bounding.GetCenter();
        Point centerRhs = obj1.Bounding.GetCenter();
        obj1.Shift(centerRhs.x - center.x, centerRhs.y - center.y);
    }
}