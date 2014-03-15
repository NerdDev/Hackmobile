using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StairElement : ThemeElement
{
    private static DrawAction<GenSpace> strokeTest = Draw.IsType<GenSpace>(GridType.Floor).
                // If not blocking a path
                And(Draw.Not(Draw.Blocking(Draw.Walkable<GenSpace>()))).
                // If there's a floor around
                And(Draw.Around(false, Draw.IsType<GenSpace>(GridType.Floor)));
    private static DrawAction<GenSpace> unitTest = Draw.IsType<GenSpace>(GridType.Floor);

    public StairElement()
    {
        SetChar('/');
    }

    public override void PreDeployTweaks(ThemeElementSpec spec)
    {
        Value2D<GenSpace> val;
        if (spec.GenGrid.GetPointAround(spec.DeployX, spec.DeployY, false, Draw.IsType<GenSpace>(GridType.StairPlace), out val))
        {
            val.x -= spec.DeployX;
            val.y -= spec.DeployY;
            if (val.x == 1)
            {
                spec.GenDeploy.Rotation = 90;
            }
            else if (val.x == -1)
            {
                spec.GenDeploy.Rotation = -90;
            }
            else if (val.y == -1)
            {
                spec.GenDeploy.Rotation = 180;
            }
        }
        if (spec.Type == GridType.StairDown)
        {
            spec.GenDeploy.Y = -1;
            spec.GenDeploy.Rotation += 180;
        }
    }

    public virtual bool Place(LayoutObject obj, Theme theme, System.Random rand, out Bounding placed)
    {
        List<Bounding> options = obj.FindRectangles(GridWidth, GridHeight, true, new StrokedAction<GenSpace>()
            {
                UnitAction = unitTest,
                StrokeAction = strokeTest
            });
        if (options.Count == 0)
        {
            placed = null;
            return false;
        }
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

