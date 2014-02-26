using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct GenSpace : IGridSpace 
{
    private GridType _type;
    public GridType Type { get { return _type; } set { _type = value; } }
    private Theme _theme;
    public Theme Theme { get { return _theme; } set { _theme = value; } }

    public GenSpace(GridType type, Theme theme)
    {
        _type = type;
        _theme = theme;
    }

    public GenSpace(GenSpace rhs)
        : this (rhs._type, rhs._theme)
    {

    }
}

public static class IGridSpaceExt
{
    public static GridType GetGridType(this IGridSpace space)
    {
        if (space == null) return GridType.NULL;
        return space.Type;
    }
}

