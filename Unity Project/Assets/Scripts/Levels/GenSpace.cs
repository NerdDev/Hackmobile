using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GenSpace : IGridSpace
{
    public GridType Type { get; set; }
    public Theme Theme { get; set; }
    public List<GenDeploy> Deploys;
    public int ThemeElementCount
    {
        get
        {
            if (Deploys == null) return 0;
            return Deploys.Count;
        }
    }
    public List<GenDeploy> MainDeploys;
    public int X { get; protected set; }
    public int Y { get; protected set; }
    public bool Walkable
    {
        get { return this.Walkable(); }
    }


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

    public IEnumerable<ThemeElement> GetThemeElements()
    {
        if (Deploys != null)
        {
            foreach (var deploy in Deploys)
            {
                yield return deploy.Element;
            }
        }
    }
}