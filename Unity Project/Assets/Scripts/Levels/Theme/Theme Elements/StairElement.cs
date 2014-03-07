using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StairElement : ThemeElement
{
    public override List<GenDeploy> PreDeployTweaks(ThemeElementSpec spec)
    {
        Value2D<GenSpace> val;
        if (spec.Grid.GetPointAround(spec.X, spec.Y, false, Draw.IsType<GenSpace>(GridType.StairPlace), out val))
        {
            val.x -= spec.X;
            val.y -= spec.Y;
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
        return null;
    }
}

