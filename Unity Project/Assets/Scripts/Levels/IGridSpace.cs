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

    public static GridType GetGridType(this IGridSpace space)
    {
        if (space == null) return GridType.NULL;
        return space.Type;
    }
}