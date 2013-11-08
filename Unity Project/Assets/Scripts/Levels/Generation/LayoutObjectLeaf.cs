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
            ShiftP.Shift(minimizeShift);
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

    public void CircularStrokeAndFill(GridType stroke, GridType fill, int centerX, int centerY, int radius)
    {
        int radiusError = 1 - radius;
        int maxRadius = radius;

        for (int y = 0; y < radius; y++)
        {
            putRow(fill, centerX - radius, centerX + radius, y + centerY);
            putRow(fill, centerX - radius, centerX + radius, centerY - y);
            ToLog(Logs.LevelGenMain);
        }
        ToLog(Logs.LevelGenMain);

        //while (maxRadius >= y)
        //{
        //    //if (x >= y)
        //    //{
        //    //    put(stroke, x + centerX, y + centerY);
        //    //    put(stroke, y + centerX, x + centerY);
        //    //    put(stroke, -x + centerX, y + centerY);
        //    //    put(stroke, -y + centerX, x + centerY);
        //    //    put(stroke, -x + centerX, -y + centerY);
        //    //    put(stroke, -y + centerX, -x + centerY);
        //    //    put(stroke, x + centerX, -y + centerY);
        //    //    put(stroke, y + centerX, -x + centerY);
        //    //}

        //    putRow(fill, centerX - x, centerX + x, y + centerY);
        //    putRow(fill, centerX - x, centerX + x, centerY - y);
        //    //putCol(fill, centerY - x, centerY + y, y + centerX);
        //    //putCol(fill, centerY - x, centerY + x, centerX - y);

        //    y++;

        //    if (radiusError < 0)
        //    {
        //        radiusError += 2 * y + 1;
        //    }
        //    else
        //    {
        //        x--;
        //        radiusError += 2 * (y - x + 1);
        //    }
        //    ToLog(Logs.LevelGenMain);
        //}
        //ToLog(Logs.LevelGenMain);
    }
    //public void CircularStrokeAndFill(GridType stroke, GridType fill, int centerX, int centerY, int radius)
    //{
    //    radius = 20;
    //    int x = radius, y = 0;
    //    int radiusError = 1 - x;
    //    int maxRadius = radius;
    //    if (radius % 2 > 0)
    //        maxRadius += 1;

    //    while (maxRadius >= y)
    //    {
    //        if (x >= y)
    //        {
    //            put(stroke, x + centerX, y + centerY);
    //            put(stroke, y + centerX, x + centerY);
    //            put(stroke, -x + centerX, y + centerY);
    //            put(stroke, -y + centerX, x + centerY);
    //            put(stroke, -x + centerX, -y + centerY);
    //            put(stroke, -y + centerX, -x + centerY);
    //            put(stroke, x + centerX, -y + centerY);
    //            put(stroke, y + centerX, -x + centerY);
    //        }

    //        putRow(fill, centerX - x, centerX + x, y + centerY);
    //        putRow(fill, centerX - x, centerX + x, centerY - y);
    //        putCol(fill, centerY - x, centerY + y, y + centerX);
    //        putCol(fill, centerY - x, centerY + x, centerX - y);

    //        y++;

    //        if (radiusError < 0)
    //        {
    //            radiusError += 2 * y + 1;
    //        }
    //        else
    //        {
    //            x--;
    //            radiusError += 2 * (y - x + 1);
    //        }
    //        ToLog(Logs.LevelGenMain);
    //    }
    //    ToLog(Logs.LevelGenMain);
    //}
    #endregion FillMethods

    public override bool ContainsPoint(Value2D<GridType> val)
    {
        return grids.ContainsPoint(val);
    }

    public override string GetTypeString()
    {
        return "Layout Object Leaf";
    }
}
