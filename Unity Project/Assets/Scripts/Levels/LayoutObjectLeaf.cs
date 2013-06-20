using System.Collections;

public class LayoutObjectLeaf : LayoutObject {

    GridMap grids = new GridMap();
    private Bounding bound = new Bounding();

    #region GetSet
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
        bound.absorb(x, y);
    }

    public void putRow(GridType t, int xl, int xr, int y)
    {
        xl -= shiftP.x;
        xr -= shiftP.x;
        y -= shiftP.y;
        bound.absorb(xl, y);
        bound.absorb(xr, y);
        grids.putRow(t, xl, xr, y);
    }

    public void putCol(GridType t, int y1, int y2, int x)
    {
        x -= shiftP.x;
        y1 -= shiftP.y;
        y2 -= shiftP.y;
        bound.absorb(x, y1);
        bound.absorb(x, y2);
        grids.putCol(t, y1, y2, x);
    }

    void putInternal(GridType t, int x, int y)
    {
        grids.put(t, x, y);
    }

    public override GridMap getMap()
    {
        return grids;
    }

    public override GridMap getBakedMap()
    {
        return new GridMap(grids, shiftP);
    }

    protected override Bounding getBoundsInternal()
    {
		return bound;	
	}
    #endregion GetSet

    #region FillMethods
    public void BoxStroke(GridType t, int width, int height)
    {
        BoxStroke(t, 0, width - 1, 0, height - 1);
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
        BoxStrokeAndFill(stroke, fill, 0, width - 1, 0, height - 1);
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

    public void BoxFill(GridType t, int width, int height)
    {
        BoxFill(t, 0, width - 1, 0, height - 1);
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
