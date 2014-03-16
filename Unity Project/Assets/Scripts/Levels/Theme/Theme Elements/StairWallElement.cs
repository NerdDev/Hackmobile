using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StairWallElement : StairElement
{
    private static DrawAction<GenSpace> strokeTest = Draw.Around(false, Draw.IsType<GenSpace>(GridType.Wall));
    private static DrawAction<GenSpace> unitTest = Draw.IsType<GenSpace>(GridType.NULL);

    public override bool Place(Container2D<GenSpace> grid, LayoutObject obj, Theme theme, Random rand, out Bounding placed)
    {
        int max = Math.Max(GridWidth, GridHeight);
        List<Bounding> options = grid.FindRectangles(GridWidth, GridHeight, true, unitTest, obj.Bounding.Expand(max));
        //options = new List<Bounding>(options.Filter((bounds) =>
        //{
        //    Counter counter = new Counter();
        //    grid.DrawRect(bounds, )
        //}));
        if (options.Count == 0)
        {
            placed = null;
            return false;
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Options:");
            foreach (Bounding bounds in options)
            {
                MultiMap<GenSpace> tmp = new MultiMap<GenSpace>();
                tmp.PutAll(obj);
                tmp.DrawRect(bounds, Draw.SetTo(GridType.INTERNAL_RESERVED_CUR, theme));
                tmp.ToLog(Logs.LevelGen);
            }
        }
        #endregion
        foreach (Bounding bounds in options)
        {
            MultiMap<GenSpace> tmp = new MultiMap<GenSpace>();
            tmp.PutAll(obj);
            tmp.DrawRect(bounds, Draw.SetTo(GridType.INTERNAL_RESERVED_CUR, theme));
            tmp.ToLog(Logs.LevelGen);
        }
        placed = null;
        return false;
        // Place startpoints
        placed = options.Random(rand);
        placed.Expand(1);
        GridLocation side = GridLocation.BOTTOMRIGHT;
        foreach (GridLocation loc in GridLocationExt.Dirs().Randomize(rand))
        {
            if (obj.DrawEdge(placed, loc, unitTest, false))
            {
                side = loc;
                break;
            }
        }
        obj.DrawEdge(placed, side, Draw.SetTo(GridType.StairPlace, theme), false);
        return true;
    }
}

