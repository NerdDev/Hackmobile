using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class WallElement : ThemeElement
{
    static DrawAction<GenSpace> _test = Draw.IsType<GenSpace>(GridType.Wall).Or(Draw.IsType<GenSpace>(GridType.Door));
    public bool PlaceFloor = true;
    public WallElement()
    {
        SetChar('#');
    }

    public override void PreDeployTweaks(ThemeElementSpec spec)
    {
        GridDirection dir;
        GridLocation loc;
        if (spec.GenGrid.AlternatesSides(spec.DeployX, spec.DeployY, _test, out dir))
        {
            spec.GenDeploy.Element = spec.Theme.Core.ThinWall.Random(spec.Random);
            spec.GenDeploy.RotateToPoint(dir, spec.Random);
        }
        else if (spec.GenGrid.Cornered(spec.DeployX, spec.DeployY, _test, out loc, false))
        {
            spec.GenDeploy.Element = spec.Theme.Core.CornerWall.Random(spec.Random);
            spec.GenDeploy.RotateToPoint(loc.Clockwise());
        }
        else if (spec.GenGrid.TShape(spec.DeployX, spec.DeployY, _test, out loc))
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

