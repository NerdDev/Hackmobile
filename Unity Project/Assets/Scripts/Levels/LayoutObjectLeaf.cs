using System.Collections;
using System.Collections.Generic;
using System;

public class LayoutObjectLeaf : LayoutObject {

    GridArray grids;
	Bounding bound = new Bounding();

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
        bound.absorb(x, y);
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
        bound.absorb(xl, y);
        bound.absorb(xr, y);
        grids.PutRow(t, xl, xr, y);
    }

    public void putCol(GridType t, int y1, int y2, int x)
    {
        x -= shiftP.x;
        y1 -= shiftP.y;
        y2 -= shiftP.y;
        bound.absorb(x, y1);
        bound.absorb(x, y2);
        grids.PutCol(t, y1, y2, x);
    }

    void putInternal(GridType t, int x, int y)
    {
        grids.Put(t, x, y);
    }

    public override GridArray getMap()
    {
        return grids;
    }

    public override GridArray getBakedMap()
    {
		if (!shiftP.isZero()){
        	return new GridArray(grids, shiftP);
		} else {
			return grids;	
		}
    }

    protected override Bounding getBoundsInternal()
    {
		return bound;
	}
    #endregion GetSet

    #region SpecGet
    public GridMap getType(GridType t)
    {
        GridMap ret = new GridMap();
        foreach (Value2D<GridType> val in grids)
        {
            if (t == val.val)
            {
				val.x += shiftP.x;
				val.y += shiftP.y;
                ret.Put(val);
            }
        }
        return ret;
    }

    public GridMap getTypes(params GridType[] ts)
    {
        return getTypes(new HashSet<GridType>(ts));
    }

    public GridMap getTypes(HashSet<GridType> ts)
    {
        GridMap ret = new GridMap();
        foreach (Value2D<GridType> val in grids)
        {
            if (ts.Contains(val.val))
            {
				val.x += shiftP.x;
				val.y += shiftP.y;
                ret.Put(val);
            }
        }
        return ret;
    }

    public GridMap getCorneredBy(GridType target, params GridType[] by)
    {
        return getCorneredBy(target, new HashSet<GridType>(by));
    }

    public GridMap getCorneredBy(GridType target, HashSet<GridType> by)
    {
        GridMap ret = new GridMap();
        GridMap targets = getType(target);
        GridMap cornerOptions = getTypes(by);
        foreach (Value2D<GridType> tval in targets)
        {
            bool corneredHoriz = cornerOptions.Contains(tval.x + 1, tval.y)
                || cornerOptions.Contains(tval.x - 1, tval.y);
            bool corneredVert = cornerOptions.Contains(tval.x, tval.y + 1)
                || cornerOptions.Contains(tval.x, tval.y - 1);
            if (corneredHoriz && corneredVert)
            {
                ret.Put(tval);
            }
        }
        return ret;
    }
    #endregion

    #region FillMethods
    public void BoxStroke(GridType t, int width, int height)
    {
		int centerShiftX = width / 2;
		int centerShiftY = height / 2;
        BoxStroke(t, - centerShiftX, width - centerShiftX - 1
			, -centerShiftY, height - centerShiftY - 1);
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
		int centerShiftX = width / 2;
		int centerShiftY = height / 2;
        BoxStrokeAndFill(stroke, fill, - centerShiftX, width - centerShiftX - 1
			, -centerShiftY, height - centerShiftY - 1);
    }

    public void BoxStrokeAndFill(GridType stroke, GridType fill, int xl, int xr, int yb, int yt)
    {
        xl -= shiftP.x;
        xr -= shiftP.x;
        yb -= shiftP.y;
        yt -= shiftP.y;
        bound.absorbX(xl);
        bound.absorbX(xr);
        bound.absorbY(yb);
        bound.absorbY(yt);
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
		int centerShiftX = width / 2;
		int centerShiftY = height / 2;
        BoxFill(t, - centerShiftX, width - centerShiftX - 1
			, -centerShiftY, height - centerShiftY - 1);
    }

    public void BoxFill(GridType t, int xl, int xr, int yb, int yt)
    {
        xl -= shiftP.x;
        xr -= shiftP.x;
        yb -= shiftP.y;
        yt -= shiftP.y;
        bound.absorbX(xl);
        bound.absorbX(xr);
        bound.absorbY(yb);
        bound.absorbY(yt);
        grids.putSquare(t, xl, xr, yb, yt);
    }
    #endregion FillMethods
}
