using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IGridType
{
    GridType Type { get; set; }
}

public interface IGridSpace :  IGridType
{
    Theme Theme { get; set; }
    int X { get; }
    int Y { get; }
    int ThemeElementCount { get; }
    IEnumerable<ThemeElement> GetThemeElements();
}

public static class IGridSpaceExt
{
    public static char GetChar(this IGridSpace space)
    {
        foreach (ThemeElement elem in space.GetThemeElements())
        {
            if (!string.IsNullOrEmpty(elem.PrintChar))
            {
                return elem.PrintChar[0];
            }
        }
        return GridTypeEnum.Convert(space.Type);
    }

    public static GridType GetGridType(this IGridType space)
    {
        if (space == null) return GridType.NULL;
        return space.Type;
    }

    public static bool Walkable(this IGridSpace space)
    {
        if (!GridTypeEnum.Walkable(space.Type)) return false;
        foreach (ThemeElement element in space.GetThemeElements())
        {
            if (!element.Walkable) return false;
        }
        return true;
    }
}