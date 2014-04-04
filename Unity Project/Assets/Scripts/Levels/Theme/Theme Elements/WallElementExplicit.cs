using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class WallElementExplicit : WallElement
{
    public ThemeQualitySet ThinWall;
    public ThemeQualitySet TWall;
    public ThemeQualitySet TEdgeWall;
    public ThemeQualitySet EdgeWall;
    public ThemeQualitySet PWall;
    public ThemeQualitySet QuadWall;
    public ThemeQualitySet CornerWall;
    public ThemeQualitySet DualInverseCornerWall;
    public ThemeQualitySet TriWall;
    public ThemeQualitySet InverseCornerWall;
    public ThemeQualitySet CornerWallFilled;
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

    public override void PreDeployTweaks(ThemeElementSpec spec)
    {
        if (spec.DeployX == 8 && spec.DeployY == 24)
        {
            int wer = 23;
            wer++;
        }
        GridLocationResults results = spec.GenGrid.DrawLocationsAroundResults(spec.DeployX, spec.DeployY, true, _test);
        GridDirection dir;
        GridLocation loc;
        bool placeFloor = PlaceFloor;
        if (spec.GenGrid.AlternatesSides(results, out dir))
        {
            spec.GenDeploy.Element = ThinWall.Get(spec.Random);
            spec.GenDeploy.RotateToPoint(dir, spec.Random);
        }
        else if (spec.GenGrid.Cornered(results, out loc, false))
        {
            if (results[loc])
            {
                spec.GenDeploy.Element = CornerWallFilled.Get(spec.Random);
            }
            else
            {
                spec.GenDeploy.Element = CornerWall.Get(spec.Random);
            }
            spec.GenDeploy.RotateToPoint(loc.Clockwise());
        }
        else if (spec.GenGrid.TShape(results, out loc))
        {
            if (results[loc.Clockwise()] && results[loc.CounterClockwise()])
            {
                spec.GenDeploy.Element = EdgeWall.Get(spec.Random);
            }
            else if (results[loc.Clockwise()] || results[loc.CounterClockwise()])
            {
                spec.GenDeploy.Element = PWall.Get(spec.Random);
                if (results[loc.Clockwise()])
                {
                    spec.GenDeploy.XScale *= -1;
                }
            }
            else
            {
                spec.GenDeploy.Element = TWall.Get(spec.Random);
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
                    spec.GenDeploy.Element = QuadWall.Get(spec.Random);
                    spec.GenDeploy.Rotate(spec.Random.NextRotation());
                    break;
                case 1:
                    spec.GenGrid.GetCorner(results, out loc);
                    spec.GenDeploy.Element = TriWall.Get(spec.Random);
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
                        spec.GenDeploy.Element = DualInverseCornerWall.Get(spec.Random);
                        spec.GenDeploy.RotateToPoint(loc.Clockwise(), spec.Random);
                    }
                    else
                    {
                        spec.GenDeploy.Element = TEdgeWall.Get(spec.Random);
                        spec.GenDeploy.RotateToPoint(loc2.Opposite());
                    }
                    break;
                case 3:
                    spec.GenGrid.GetCorner(!results, out loc);
                    spec.GenDeploy.Element = InverseCornerWall.Get(spec.Random);
                    spec.GenDeploy.RotateToPoint(loc.Clockwise());
                    break;
                case 4:
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
