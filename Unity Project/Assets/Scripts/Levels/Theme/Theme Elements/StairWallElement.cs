using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StairWallElement : StairElement
{
    public bool InwardStairs = true;
    private static DrawAction<GenSpace> frontTest = Draw.IsType<GenSpace>(GridType.Wall);
    private static DrawAction<GenSpace> unitTest = Draw.IsType<GenSpace>(GridType.NULL);

    public override void PreDeployTweaks(ThemeElementSpec spec)
    {
        GridLocation loc = spec.GenGrid.FindEdges(spec.Bounding.Expand(1), Draw.IsType<GenSpace>(GridType.StairPlace), false, true).First();
        CenterDoodad(spec);
        spec.GenDeploy.RotateToPoint(loc);
        if (spec.Type == GridType.StairDown)
        {
            spec.GenDeploy.Y -= StairHeight;
            spec.GenDeploy.YRotation += 180;
        }
    }

    public override bool Place(Container2D<GenSpace> grid, LayoutObject<GenSpace> obj, Theme theme, Random rand, out Boxing placed)
    {
        int max = Math.Max(GridWidth, GridLength);
        List<Boxing> options;
        if (InwardStairs)
        {
            throw new NotImplementedException();
        }
        else
        {
            options = new List<Boxing>(
                grid.FindBoxes(
                    GridWidth,
                    GridLength,
                    GridLocation.TOP,
                    new BoxedAction<GenSpace>(
                        frontTest.And(Draw.PointContainedIn(obj)),
                        unitTest),
                        true,
                        true,
                        obj.Bounding.Expand(max))
                .Filter((box) =>
                {
                    box.Expand(1);
                    Counter counter = new Counter();
                    bool ret = grid.DrawEdge(box, box.Front, Draw.Or(Draw.IsType<GenSpace>(GridType.Floor).And(Draw.Count<GenSpace>(counter)), Draw.Walkable<GenSpace>()), false);
                    DrawAction<GenSpace> edgeTest = Draw.IsType<GenSpace>(GridType.NULL, GridType.Wall);
                    ret = ret && grid.DrawEdge(box, box.Front.Opposite(), edgeTest, true);
                    ret = ret && grid.DrawEdge(box, box.Front.Clockwise90(), edgeTest, false);
                    ret = ret && grid.DrawEdge(box, box.Front.CounterClockwise90(), edgeTest, false);
                    box.Expand(-1);
                    #region DEBUG
                    if (!ret && BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
                    {
                        MultiMap<GenSpace> tmp = new MultiMap<GenSpace>();
                        tmp.PutAll(obj);
                        tmp.DrawRect(box, Draw.SetTo(GridType.INTERNAL_RESERVED_CUR, theme));
                        tmp.DrawEdge(box, box.Front, Draw.SetTo(GridType.INTERNAL_RESERVED_BLOCKED, theme));
                        tmp.ToLog(Logs.LevelGen, "Failed Option");
                    }
                    #endregion
                    return ret && counter > 0;
                }));
        }
        if (options.Count == 0)
        {
            placed = null;
            return false;
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Options:");
            foreach (Boxing boxing in options)
            {
                MultiMap<GenSpace> tmp = new MultiMap<GenSpace>();
                tmp.PutAll(obj);
                tmp.DrawRect(boxing, Draw.SetTo(GridType.INTERNAL_RESERVED_CUR, theme));
                tmp.DrawEdge(boxing, boxing.Front, Draw.SetTo(GridType.INTERNAL_RESERVED_BLOCKED, theme));
                tmp.ToLog(Logs.LevelGen);
            }
        }
        #endregion
        // Place startpoints
        placed = options.Random(rand);
        obj.DrawEdge(placed, placed.Front, Draw.Around(false, Draw.IsType<GenSpace>(GridType.Floor).IfThen(Draw.SetTo(GridType.StairPlace, theme))));
        return true;
    }
}

