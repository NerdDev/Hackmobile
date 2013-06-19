using System.Collections;
using System.Collections.Generic;
using System;

public class LayoutObjectLeaf : LayoutObject {

    GridArray grids;

    public LayoutObjectLeaf(int width, int height)
    {
        grids = new GridArray(width, height);
    }

    #region GetSet
    public GridType get(int x, int y)
    {
        x += shiftP.x;
        y += shiftP.y;
        return grids.Get(x, y);
    }

    public void put(GridType t, int x, int y)
    {
        x -= shiftP.x;
        y -= shiftP.y;
        putInternal(t, x, y);
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
        xl -= shiftP.x;
        xr -= shiftP.x;
        y -= shiftP.y;
        grids.PutRow(t, xl, xr, y);
    }

    public void putCol(GridType t, int y1, int y2, int x)
    {
        x -= shiftP.x;
        y1 -= shiftP.y;
        y2 -= shiftP.y;
        grids.PutCol(t, y1, y2, x);
    }

    void putInternal(GridType t, int x, int y)
    {
        grids.Put(t, x, y);
    }

    public override GridArray GetArray()
    {
        return grids;
    }

    public override GridArray GetBakedArray()
    {
		if (!shiftP.isZero()){
        	return new GridArray(grids, shiftP);
		} else {
			return grids;	
		}
    }

    protected override Bounding GetBoundingInternal()
    {
		return grids.GetBoundingInternal();
	}
    #endregion GetSet

    #region FillMethods
    protected void BoxStroke(GridType t, int width, int height)
    {
        int centerX = grids.getWidth() / 2;
        int centerY = grids.getHeight() / 2;
        BoxStroke(t, centerX - width / 2, centerX + width / 2 - 1,
            centerY - height / 2, centerY + height / 2 - 1);
    }

    protected void BoxStroke(GridType t, int xl, int xr, int yb, int yt)
    {
        putRow(t, xl, xr, yb);
        putRow(t, xl, xr, yt);
        yb++;
        yt--;
        putCol(t, yb, yt, xl);
        putCol(t, yb, yt, xr);
    }

    protected void BoxStrokeAndFill(GridType stroke, GridType fill, int width, int height)
    {
        BoxStrokeAndFill(stroke, fill, 0, width - 1, 0, height - 1);
    }

    protected void BoxStrokeAndFill(GridType stroke, GridType fill, int xl, int xr, int yb, int yt)
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

    protected void BoxFill(GridType t, int width, int height)
    {
        int centerX = grids.getWidth() / 2;
        int centerY = grids.getHeight() / 2;
        BoxFill(t, centerX - width / 2, centerX + width / 2 - 1,
            centerY - height / 2, centerY + height / 2 - 1);
    }

    protected void BoxFill(GridType t, int xl, int xr, int yb, int yt)
    {
        grids.putSquare(t, xl, xr, yb, yt);
    }
    #endregion FillMethods
}
