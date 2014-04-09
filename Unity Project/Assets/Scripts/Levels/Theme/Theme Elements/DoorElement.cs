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
        if (spec.DeployX == -26 && spec.DeployY == -14)
        {
            int wer = 23;
            wer++;
        }
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
        else if (spec.GenGrid.AlternatesCorners(spec.DeployX, spec.DeployY, Draw.IsType<GenSpace>(GridType.Wall), out walkableDir))
        {
            spec.GenDeploy.RotateToPoint(walkableDir.Rotate90(), spec.Random);
        }
        // Offset alternates
        else if (spec.GenGrid.AlternateSidesOffset(spec.DeployX, spec.DeployY, Draw.IsType<GenSpace>(GridType.Wall), out offsetLocation))
        {
            PlaceFlush(spec.GenDeploy, offsetLocation.Clockwise90());
        }
        PlaceFloors(spec);
    }


}

