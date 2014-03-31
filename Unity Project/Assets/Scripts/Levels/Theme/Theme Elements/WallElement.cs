using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class WallElement : ThemeElement
{
    static DrawAction<GenSpace> _test = (arr, x, y) =>
    {
        GenSpace space;
        if (!arr.TryGetValue(x, y, out space)) return true;
        switch (arr[x, y].Type)
        {
            case GridType.Wall:
            case GridType.Door:
            case GridType.StairDown:
            case GridType.StairUp:
                return true;
            default:
                return false;
        }
    };
    public bool PlaceFloor = true;
    public WallElement()
    {
        SetChar('#');
    }

    protected virtual void HandleAlternateSides(ThemeElementSpec spec, GridDirection dir)
    {
        spec.GenDeploy.Element = spec.Theme.Core.ThinWall.Random(spec.Random);
        spec.GenDeploy.RotateToPoint(dir, spec.Random);
    }

    public override void PreDeployTweaks(ThemeElementSpec spec)
    {
        GridLocationResults results = spec.GenGrid.DrawLocationsAroundResults(spec.DeployX, spec.DeployY, true, _test);
        GridDirection dir;
        GridLocation loc;
        bool placeFloor = PlaceFloor;
        if (spec.GenGrid.AlternatesSides(results, out dir))
        {
            HandleAlternateSides(spec, dir);
        }
        else if (spec.GenGrid.Cornered(results, out loc, false))
        {
            if (results[loc])
            {
                spec.GenDeploy.Element = spec.Theme.Core.CornerWallFilled.Random(spec.Random);
            }
            else
            {
                spec.GenDeploy.Element = spec.Theme.Core.CornerWall.Random(spec.Random);
            }
            spec.GenDeploy.RotateToPoint(loc.Clockwise());
        }
        else if (spec.GenGrid.TShape(results, out loc))
        {
            if (results[loc.Clockwise()] && results[loc.CounterClockwise()])
            {
                spec.GenDeploy.Element = spec.Theme.Core.EdgeWall.Random(spec.Random);
            }
            else if (results[loc.Clockwise()] || results[loc.CounterClockwise()])
            {
                spec.GenDeploy.Element = spec.Theme.Core.PWall.Random(spec.Random);
                if (results[loc.Clockwise()])
                {
                    spec.GenDeploy.XScale *= -1;
                } 
            }
            else
            {
                spec.GenDeploy.Element = spec.Theme.Core.TWall.Random(spec.Random);
            }
            spec.GenDeploy.RotateToPoint(loc);
        }
        else
        {
            Counter numCorners = new Counter();
            spec.GenGrid.DrawCorners(spec.DeployX, spec.DeployY, _test.IfThen(Draw.Count<GenSpace>(numCorners)));
            switch (numCorners)
            {
                case 0:
                    spec.GenDeploy.Element = spec.Theme.Core.QuadWall.Random(spec.Random);
                    spec.GenDeploy.Rotate(spec.Random.NextRotation());
                    break;
                case 1:
                    spec.GenGrid.GetCorner(results, out loc);
                    spec.GenDeploy.Element = spec.Theme.Core.TriWall.Random(spec.Random);
                    spec.GenDeploy.RotateToPoint(loc.Clockwise());
                    break;
                case 2:
                    spec.GenGrid.GetCorner(results, out loc);
                    results[loc] = false;
                    GridLocation loc2;
                    spec.GenGrid.GetCorner(results, out loc2);
                    loc2 = loc2.Merge(loc);
                    if (loc2 == GridLocation.CENTER)
                    {
                        spec.GenDeploy.Element = spec.Theme.Core.DualInverseCornerWall.Random(spec.Random);
                        spec.GenDeploy.RotateToPoint(loc.Clockwise(), spec.Random);
                    }
                    else
                    {
                        spec.GenDeploy.Element = spec.Theme.Core.TEdgeWall.Random(spec.Random);
                        spec.GenDeploy.RotateToPoint(loc2.Opposite().CounterClockwise());
                    }
                    break;
                case 3:
                    spec.GenGrid.GetCorner(!results, out loc);
                    spec.GenDeploy.Element = spec.Theme.Core.InverseCornerWall.Random(spec.Random);
                    spec.GenDeploy.RotateToPoint(loc.Clockwise());
                    break;
                case 4:
                    spec.GenDeploy.Element = spec.Theme.Core.Wall.Random(spec.Random);
                    spec.GenDeploy.Rotate(spec.Random.NextRotation());
                    placeFloor = false;
                    break;
            }
        }
        if (placeFloor)
        {
            PlaceFloors(spec);
        }
    }
}

