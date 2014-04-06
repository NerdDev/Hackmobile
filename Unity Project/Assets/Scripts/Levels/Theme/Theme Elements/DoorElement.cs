using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DoorElement : ThemeElement
{
    public DoorElement()
    {
        Walkable = true;
    }

    public override void PreDeployTweaks(ThemeElementSpec spec)
    {
        // Normal 
        GridDirection walkableDir;
        GridLocation offsetLocation;
        if (spec.GenGrid.AlternatesSides(spec.DeployX, spec.DeployY, Draw.IsType<GenSpace>(GridType.Wall), out walkableDir))
        {
            bool neg = spec.Random.NextBool();
            if (walkableDir == GridDirection.HORIZ)
            {
                spec.GenDeploy.Rotate(spec.Random.NextFlip());
            }
            else
            {
                spec.GenDeploy.Rotate(spec.Random.NextClockwise());
            }
        }
        // Diagonal door
        else if (spec.GenGrid.AlternatesCorners(spec.DeployX, spec.DeployY, Draw.Walkable(), out walkableDir))
        {
            spec.GenDeploy.YRotation = walkableDir == GridDirection.DIAGTLBR ? -45 : 45;
        }
        // Offset alternates
        else if (spec.GenGrid.AlternateSidesOffset(spec.DeployX, spec.DeployY, Draw.Not(Draw.Walkable()), out offsetLocation))
        {
            PlaceFlush(spec.GenDeploy, offsetLocation);
        }
        PlaceFloors(spec);
    }


}

