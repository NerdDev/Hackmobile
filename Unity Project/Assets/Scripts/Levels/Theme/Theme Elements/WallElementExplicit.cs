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
    public ThemeQualitySet DiagonalSmallWall;
    public ThemeQualitySet DiagonalBluntWall;
    public ThemeQualitySet DiagonalWedgeWall;
    public ThemeQualitySet DiagonalTurnWall;
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

    protected bool IsDiagonalPiece(ThemeElementSpec spec, GridLocationResults results, out GridLocation loc)
    {
        if (!results.Cornered(out loc, false)) return false;
        return results[loc.Clockwise90()] || results[loc.CounterClockwise90()];
    }

    public override void PreDeployTweaks(ThemeElementSpec spec)
    {
        if (spec.DeployX == -25 && spec.DeployY == -13)
        {
            int wer = 23;
            wer++;
        }
        GridLocationResults results = spec.GenGrid.DrawLocationsAroundResults(spec.DeployX, spec.DeployY, true, _test);
        GridDirection dir;
        GridLocation loc, loc2;
        bool placeFloor = PlaceFloor;
        if (results.AlternatesSides(out dir))
        {
            spec.GenDeploy.Element = ThinWall.Get(spec.Random);
            spec.GenDeploy.RotateToPoint(dir, spec.Random);
        }
        else if (IsDiagonalPiece(spec, results, out loc))
        {
            if (results[loc.Opposite()])
            {
                spec.GenDeploy.Element = DiagonalWedgeWall.Get(spec.Random);
            }
            else
            {
                spec.GenDeploy.Element = DiagonalSmallWall.Get(spec.Random);
            }
            spec.GenDeploy.RotateToPoint(loc.Clockwise());
        }
        else if (results.Cornered(out loc, false))
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
        else if (results.TShape(out loc))
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
        else if (results.IsTurn(out loc, out loc2))
        {
            spec.GenDeploy.Element = DiagonalTurnWall.Get(spec.Random);
            spec.GenDeploy.RotateToPoint(loc2);
            if (results[loc.Opposite().Clockwise()])
            {
                spec.GenDeploy.XScale *= -1;
            }
        }
        else
        {
            switch (results.NumCorners)
            {
                case 0:
                    spec.GenDeploy.Element = QuadWall.Get(spec.Random);
                    spec.GenDeploy.Rotate(spec.Random.NextRotation());
                    break;
                case 1:
                    results.GetCorner(out loc);
                    spec.GenDeploy.Element = TriWall.Get(spec.Random);
                    spec.GenDeploy.RotateToPoint(loc.Clockwise());
                    break;
                case 2:
                    results.GetCorner(out loc);
                    results[loc] = false;
                    results.GetCorner(out loc2);
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
                    (!results).GetCorner(out loc);
                    int neighborX, neighborY;
                    loc.Clockwise().Modify(spec.DeployX, spec.DeployY, out neighborX, out neighborY);
                    GridLocationResults neighborResults = spec.GenGrid.DrawLocationsAroundResults(neighborX, neighborY, true, _test);
                    loc.CounterClockwise().Modify(spec.DeployX, spec.DeployY, out neighborX, out neighborY);
                    GridLocationResults neighborResults2 = spec.GenGrid.DrawLocationsAroundResults(neighborX, neighborY, true, _test);
                    GridLocation tmp;
                    if (IsDiagonalPiece(spec, neighborResults, out tmp) || IsDiagonalPiece(spec, neighborResults2, out tmp))
                    {
                        spec.GenDeploy.Element = DiagonalBluntWall.Get(spec.Random);
                    }
                    else
                    {
                        spec.GenDeploy.Element = InverseCornerWall.Get(spec.Random);
                    }
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
