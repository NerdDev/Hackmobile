using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DoorElement : ThemeElement
{
    public ThemeElement DiagonalExtensions;

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
            CenterDoodad(spec);
        }
        // Diagonal door
        else if (spec.GenGrid.AlternatesCorners(spec.DeployX, spec.DeployY, Draw.IsType<GenSpace>(GridType.Wall), out walkableDir))
        {
            spec.GenDeploy.RotateToPoint(walkableDir.Rotate90());
            if (DiagonalExtensions != null)
            {
                GenDeploy extensions = new GenDeploy(DiagonalExtensions);
                extensions.RotateToPoint(walkableDir.Rotate90());
                spec.AddAdditional(extensions, spec.DeployX, spec.DeployY);
            }
        }
        // Offset alternates
        else if (spec.GenGrid.AlternateSidesOffset(spec.DeployX, spec.DeployY, Draw.IsType<GenSpace>(GridType.Wall), out offsetLocation))
        {
            PlaceFlush(spec.GenDeploy, offsetLocation.Clockwise90());
        }
        PlaceFloors(spec);
    }


}

