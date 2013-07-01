using System.Collections;
using System.Collections.Generic;
using System;

public class LayoutObjectLeaf : LayoutObject {

    protected GridArray grids;

    public LayoutObjectLeaf(int width, int height)
    {
        grids = new GridArray(width, height);
    }

    #region GetSet
    public GridType get(int x, int y)
    {
        return grids[x, y];
    }

    public void put(GridType t, int x, int y)
    {
        grids[x,y] = t;
    }

    public void putAll(GridMap map)
    {
        foreach (Value2D<GridType> vals in map)
        {
            put(vals.val, vals.x, vals.y);
        }
    }

    public void putRow(GridType t, int xl, int xr, int y)
    {
        grids.PutRow(t, xl, xr, y);
    }

    public void putCol(GridType t, int y1, int y2, int x)
    {
        grids.PutCol(t, y1, y2, x);
    }

    public override GridArray GetArray()
    {
        return grids;
    }

    protected override Bounding GetBoundingInternal()
    {
		return grids.GetBoundingInternal();
	}
    #endregion GetSet

    #region FillMethods
    public void BoxStroke(GridType t, int width, int height)
    {
        int centerX = grids.getWidth() / 2;
        int centerY = grids.getHeight() / 2;
        BoxStroke(t, centerX - width / 2, centerX + width / 2 - 1,
            centerY - height / 2, centerY + height / 2 - 1);
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

    public void BoxStrokeAndFill(GridType stroke, GridType fill, int width, int height)
    {
        int centerX = grids.getWidth() / 2;
        int centerY = grids.getHeight() / 2;
        BoxStrokeAndFill(stroke, fill, centerX - width / 2, centerX + width / 2 - 1,
            centerY - height / 2, centerY + height / 2 - 1);
    }

    public void BoxStrokeAndFill(GridType stroke, GridType fill, int xl, int xr, int yb, int yt)
    {
        grids.PutRow(stroke, xl, xr, yb);
        grids.PutRow(stroke, xl, xr, yt);
        yb++;
        yt--;
        grids.PutCol(stroke, yb, yt, xl);
        grids.PutCol(stroke, yb, yt, xr);
        xl++;
        xr--;
        grids.putSquare(fill, xl, xr, yb, yt);
    }

    public void BoxFill(GridType t, int width, int height)
    {
        int centerX = grids.getWidth() / 2;
        int centerY = grids.getHeight() / 2;
        BoxFill(t, centerX - width / 2, centerX + width / 2 - 1,
            centerY - height / 2, centerY + height / 2 - 1);
    }

    public void BoxFill(GridType t, int xl, int xr, int yb, int yt)
    {
        grids.putSquare(t, xl, xr, yb, yt);
    }
    #endregion FillMethods
}
