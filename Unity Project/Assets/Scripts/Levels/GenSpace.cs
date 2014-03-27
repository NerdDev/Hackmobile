using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GenSpace : IGridSpace
{
    public GridType Type { get; set; }
    public Theme Theme { get; set; }
    public List<GenDeploy> Deploys;
    public List<GenDeploy> MainDeploys;
    public int X { get; protected set; }
    public int Y { get; protected set; }

    public GenSpace(GridType type, Theme theme, int x, int y)
    {
        Type = type;
        Theme = theme;
        Deploys = null;
        this.X = x;
        this.Y = y;
    }

    public void AddDeploy(GenDeploy elem, int x, int y)
    {
        if (Deploys == null)
        {
            Deploys = new List<GenDeploy>(3);
        }
        if (MainDeploys == null)
        {
            MainDeploys = new List<GenDeploy>(3);
        }
        if (elem.OriginPt == null)
        {
            elem.OriginPt = new Point(x, y);
            MainDeploys.Add(elem);
        }
        Deploys.Add(elem);
        elem.Spaces[x, y] = this;
    }

    public char GetChar()
    {
        if (Deploys != null)
        {
            foreach (GenDeploy d in Deploys)
            {
                if (!string.IsNullOrEmpty(d.Element.PrintChar))
                {
                    return d.Element.PrintChar[0];
                }
            }
        }
        return GridTypeEnum.Convert(Type);
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

