using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class LayoutObject {

    MultiMap<GridType> grids = new MultiMap<GridType>();
    Point shiftP = new Point();
    Bounding bounds = new Bounding();

    public GridType get(int x, int y)
    {
        x -= shiftP.x;
        y -= shiftP.y;
        return grids.get(x, y);
    }

    public void put(GridType t, int x, int y)
    {
        x -= shiftP.x;
        y -= shiftP.y;
        putInternal(t, x, y);
        bounds.absorb(x, y);
    }
	
	void putInternal(GridType t, int x, int y)
	{
        grids.put(t, x, y);
	}
	
	public void putRow(GridType t, int xl, int xr, int y)
    {
        xl -= shiftP.x;
        xr -= shiftP.x;
        y -= shiftP.y;
        bounds.absorb(xl, y);
        bounds.absorb(xr, y);
        grids.putRow(t, xl, xr, y);
	}

    public void putCol(GridType t, int y1, int y2, int x)
    {
        x -= shiftP.x;
        y1 -= shiftP.y;
        y2 -= shiftP.y;
        bounds.absorb(x, y1);
        bounds.absorb(x, y2);
        grids.putCol(t, y1, y2, x);
    }
	
	public void shift(int x, int y)
	{
		shiftP.shift(x,y);
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

    public void BoxStroke(GridType t, int width, int height)
    {
        BoxStroke(t, 0, width, 0, height);
    }

    public void BoxStrokeAndFill(GridType stroke, GridType fill, int xl, int xr, int yb, int yt)
    {
        xl -= shiftP.x;
        xr -= shiftP.x;
        yb -= shiftP.y;
        yt -= shiftP.y;
        bounds.absorbX(xl);
        bounds.absorbX(xr);
        bounds.absorbX(yb);
        bounds.absorbX(yt);
        grids.putRow(stroke, xl, xr, yb);
        grids.putRow(stroke, xl, xr, yt);
        yb++;
        yt--;
        grids.putCol(stroke, yb, yt, xl);
        grids.putCol(stroke, yb, yt, xr);
        xl++;
        xr--;
        grids.putSquare(fill, xl, xr, yb, yt);
    }

    public void BoxStroke(GridType t, int xl, int xr, int yb, int yt)
    {
        putRow(t, xl, xr, yb);
        putRow(t, xl, xr, yt);
        yb++;
        yt--;
        putCol(t, yb, yt, xl);
        putCol(t, yb, yt, xr);
    }
	
	public void BoxFill(GridType t, int xl, int xr, int yb, int yt)
    {
        xl -= shiftP.x;
        xr -= shiftP.x;
        yb -= shiftP.y;
        yt -= shiftP.y;
        bounds.absorbX(xl);
        bounds.absorbX(xr);
        bounds.absorbX(yb);
        bounds.absorbX(yt);
        grids.putSquare(t, xl, xr, yb, yt);
	}
}
