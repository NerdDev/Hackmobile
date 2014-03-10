using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GenSpace : IGridSpace 
{
    private GridType _type;
    public GridType Type { get { return _type; } set { _type = value; } }
    private Theme _theme;
    public Theme Theme { get { return _theme; } set { _theme = value; } }
    public List<GenDeploy> Deploys;
    public int X { get; protected set; }
    public int Y { get; protected set; }

    public GenSpace(GridType type, Theme theme, int x, int y)
    {
        _type = type;
        _theme = theme;
        Deploys = null;
        this.X = x;
        this.Y = y;
    }

    public void AddDeploy(GenDeploy elem)
    {
        if (Deploys == null)
        {
            Deploys = new List<GenDeploy>(3);
        }
        elem.Spaces.Add(this);
        Deploys.Add(elem);
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

