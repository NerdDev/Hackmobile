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
        if (spec.GenGrid.AlternatesSides(spec.DeployX, spec.DeployY, Draw.Walkable<GenSpace>(), out walkableDir))
        {
            bool neg = spec.Random.NextBool();
            if (walkableDir == GridDirection.HORIZ)
            {
                //PlaceFlush(spec.GenDeploy, neg ? GridLocation.LEFT : GridLocation.RIGHT);
                spec.GenDeploy.Rotate(spec.Random.NextClockwise());
            }
            else
            {
                //PlaceFlush(spec.GenDeploy, neg ? GridLocation.BOTTOM : GridLocation.TOP);
            }
        }
        // Diagonal door
        else if (spec.GenGrid.AlternatesCorners(spec.DeployX, spec.DeployY, Draw.Walkable<GenSpace>(), out walkableDir))
        {
            spec.GenDeploy.Rotation = walkableDir == GridDirection.DIAGTLBR ? -45 : 45;
        }
        // Offset alternates
        else if (spec.GenGrid.AlternateSidesOffset(spec.DeployX, spec.DeployY, Draw.Not(Draw.Walkable<GenSpace>()), out offsetLocation))
        {
            PlaceFlush(spec.GenDeploy, offsetLocation);
        }
        PlaceFloors(spec);
    }


}

