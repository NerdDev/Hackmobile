using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class WallElement : ThemeElement
{
    public bool PlaceFloor = true;
    public WallElement()
    {
        SetChar('#');
    }

    public override void PreDeployTweaks(ThemeElementSpec spec)
    {
        GridDirection dir;
        GridLocation loc;
        DrawAction<GenSpace> draw = Draw.WallType<GenSpace>().Or(Draw.IsType<GenSpace>(GridType.Door));
        if (spec.GenGrid.AlternatesSides(spec.DeployX, spec.DeployY, draw, out dir))
        {
            spec.GenDeploy.Element = spec.Theme.Core.ThinWall.Random(spec.Random);
            spec.GenDeploy.RotateToPoint(dir, spec.Random);
        }
        else if (spec.GenGrid.Cornered(spec.DeployX, spec.DeployY, draw, out loc, false))
        {
            spec.GenDeploy.Element = spec.Theme.Core.CornerWall.Random(spec.Random);
            spec.GenDeploy.RotateToPoint(loc);
        }
        else if (spec.GenGrid.TShape(spec.DeployX, spec.DeployY, draw, out loc))
        {
            spec.GenDeploy.Element = spec.Theme.Core.TWall.Random(spec.Random);
            spec.GenDeploy.RotateToPoint(loc);
        }
        else
        {
            spec.GenDeploy.Element = spec.Theme.Core.QuadWall.Random(spec.Random);
        }
        if (PlaceFloor)
        {
            PlaceFloors(spec);
        }
    }
}

