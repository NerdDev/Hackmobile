using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StairElement : ThemeElement
{
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
}

