using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StairSpiralElement : StairElement
{
    public virtual DrawAction<GenSpace> UnitTest { get { return Draw.EmptyFloorNotBlocking<GenSpace>(); } }
    public virtual DrawAction<GenSpace> FrontTest { get { return Draw.IsType<GenSpace>(GridType.Floor); } }
    public virtual DrawAction<GenSpace> BackTest { get { return Draw.True<GenSpace>(); } }

    public override void PreDeployTweaks(ThemeElementSpec spec)
    {
        Value2D<GenSpace> val;
        if (spec.GenGrid.GetPointAround(spec.DeployX, spec.DeployY, false, Draw.IsType<GenSpace>(GridType.StairPlace), out val))
        {
            val.x -= spec.DeployX;
            val.y -= spec.DeployY;
            if (val.x == 1)
            {
                spec.GenDeploy.YRotation = 90;
            }
            else if (val.x == -1)
            {
                spec.GenDeploy.YRotation = -90;
            }
            else if (val.y == -1)
            {
                spec.GenDeploy.YRotation = 180;
            }
        }
        if (spec.Type == GridType.StairDown)
        {
            spec.GenDeploy.Y = -1;
            spec.GenDeploy.YRotation += 180;
        }
    }

    public override bool Place(Container2D<GenSpace> grid, LayoutObject<GenSpace> obj, Theme theme, System.Random rand, out Boxing placed)
    {
        List<Bounding> options = obj.FindRectangles(GridWidth, GridLength, true, UnitTest);
        options = new List<Bounding>(options.Filter((bounds) =>
        {
            Counter counter = new Counter();
            grid.DrawRect(new Bounding(bounds, 1), FrontTest.IfThen(Draw.Count<GenSpace>(counter)));
            return counter > 0;
        }));
        if (options.Count == 0)
        {
            placed = null;
            return false;
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Options:");
            if (GridWidth == 1 && GridLength == 1)
            {
                MultiMap<GenSpace> tmp = new MultiMap<GenSpace>();
                tmp.PutAll(obj);
                foreach (Bounding bounds in options)
                {
                    tmp.DrawRect(bounds, Draw.SetTo(GridType.INTERNAL_RESERVED_CUR, theme));
                }
                tmp.ToLog(Logs.LevelGen);
            }
            else
            {
                foreach (Bounding bounds in options)
                {
                    MultiMap<GenSpace> tmp = new MultiMap<GenSpace>();
                    tmp.PutAll(obj);
                    tmp.DrawRect(bounds, Draw.SetTo(GridType.INTERNAL_RESERVED_CUR, theme));
                    tmp.ToLog(Logs.LevelGen);
                }
            }
        }
        #endregion
        // Place startpoints
        placed = null;
        return false;
        //placed = options.Random(rand);
        placed.Expand(1);
        GridLocation side = GridLocation.BOTTOMRIGHT;
        foreach (GridLocation loc in GridLocationExt.Dirs().Randomize(rand))
        {
            if (obj.DrawEdge(placed, loc, UnitTest, false))
            {
                side = loc;
                break;
            }
        }
        obj.DrawEdge(placed, side, Draw.SetTo(GridType.StairPlace, theme), false);
        return true;
    }
}
