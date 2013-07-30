using System.Collections;
using System.Collections.Generic;
using System;

public class LayoutObjectLeaf : LayoutObject {

    protected GridArray grids;

    protected LayoutObjectLeaf() : base()
    {
    }

    public LayoutObjectLeaf(int width, int height)
        : this(new GridArray(width, height))
    {
    }

    public LayoutObjectLeaf(GridArray arr) : this()
    {
        grids = arr;
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

    protected override Bounding GetBoundingUnshifted()
    {
        if (finalized)
        {
            return bakedBounds;
        }
		return grids.GetBounding();
	}
    #endregion GetSet

    public override void Bake(bool shiftCompensate)
    {
        Point minimizeShift = grids.Minimize(1);
        if (shiftCompensate)
        {
            ShiftP.shift(minimizeShift);
        }
        bakedBounds = grids.GetBoundingInternal();
        finalized = true;
    }

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

    public void CircularStrokeAndFill(GridType stroke, GridType fill, int radius)
    {
        int centerX = grids.getWidth() / 2;
        int centerY = grids.getHeight() / 2;
        CircularStrokeAndFill(stroke, fill, centerX, centerY, radius);
    }
    public void CircularStrokeAndFill(GridType stroke, GridType fill, int x0, int y0, int radius)
    {
        int x = radius, y = 0;
        int radiusError = 1 - x;

        while (x >= y)
        {
            put(stroke, x + x0, y + y0);
            put(stroke, y + x0, x + y0);
            put(stroke, -x + x0, y + y0);
            put(stroke, -y + x0, x + y0);
            put(stroke, -x + x0, -y + y0);
            put(stroke, -y + x0, -x + y0);
            put(stroke, x + x0, -y + y0);
            put(stroke, y + x0, -x + y0);

            putRow(fill, x0 - x, x0 + x, y + y0);
            putRow(fill, x0 - x, x0 + x, y0 - y);
            putCol(fill, y0 - x, y0 + x, y + x0);
            putCol(fill, y0 - x, y0 + x, x0 - y);

            y++;

            if (radiusError < 0) radiusError += 2 * y + 1;
            else
            {
                x--;
                radiusError += 2 * (y - x + 1);
            }
        }
        /*for (int yy = -radius; yy <= radius; yy++)
        {
            for (int xx = -radius; xx <= radius; xx++)
            {
                if (xx * xx + yy * yy <= radius * radius) put(fill, x0 + xx, y0 + yy);
            }
        }*/
    }
    #endregion FillMethods

    public override bool ContainsPoint(Value2D<GridType> val)
    {
        return grids.ContainsPoint(val);
    }

    public override String GetTypeString()
    {
        return "Layout Object Leaf";
    }
}
