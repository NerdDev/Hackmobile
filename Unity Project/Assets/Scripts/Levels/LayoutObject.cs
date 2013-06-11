using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class LayoutObject {

    MultiMap<GridType> grids = new MultiMap<GridType>();
    Bounding bounds = new Bounding();

    public GridType get(int x, int y)
    {
        return grids.get(x, y);
    }

    public void put(GridType t, int x, int y)
    {
        grids.put(t, x, y);
        bounds.absorb(x, y);
    }

    protected string toString()
    {
        string ret = "";
        foreach (string s in toRowStrings())
        {
            ret += s + "\n";
        }
        return ret;
    }

    protected List<string> toRowStrings()
    {
        List<string> ret = new List<string>();
        for (int y = bounds.rangeMin; y <= bounds.rangeMax; y++ )
        {
            SortedDictionary<int, GridType> row = null;
            grids.getRow(y, out row);
            string rowStr = "";
            GridType t;
            for (int x = bounds.domainMin; x <= bounds.domainMax; x++)
            {
                if (row != null && row.TryGetValue(x, out t))
                {
                    rowStr += LevelGenerator.getAscii(t);
                } else {
                	rowStr += " ";
				}
            }
			ret.Add(rowStr);
        }
        return ret;
    }

    public void toLog(DebugManager.Logs log)
    {
        if (DebugManager.logging(log))
        {
            foreach (string s in toRowStrings())
            {
                DebugManager.w(log, s);
            }
        }
    }

    public void generateHorLine(GridType t, int x1, int x2, int y)
    {
        for (; x1 <= x2; x1++)
        {
            put(t, x1, y);
        }
    }

    public void generateVertLine(GridType t, int y1, int y2, int x)
    {
        for (; y1 <= y2; y1++)
        {
            put(t, x, y1);
        }
    }

    public void generateBox(GridType t, int width, int height)
    {
        generateBox(t, 0, width, 0, height);
    }

    public void generateBox(GridType t, int xl, int xr, int yb, int yt)
    {
        generateHorLine(t, xl, xr, yb);
        generateHorLine(t, xl, xr, yt);
        yb++;
        yt--;
        generateVertLine(t, yb, yt, xl);
        generateVertLine(t, yb, yt, xr);
    }
}
