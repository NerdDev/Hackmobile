using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DoorElement : ThemeElement
{
    public override void PreDeployTweaks(ThemeElementSpec spec)
    {
        spec.Space.Deploys.Add(new GridDeploy(spec.Theme.Floor.Random(spec.Random).GO));
        // Normal 
        GridDirection walkableDir;
        GridLocation offsetLocation;
        if (spec.Grid.AlternatesSides(spec.X, spec.Y, Draw.Walkable<GenSpace>(), out walkableDir))
        {
            bool neg = spec.Random.NextBool();
            if (walkableDir == GridDirection.HORIZ)
            {
                PlaceFlush(spec.GenDeploy, neg ? GridLocation.LEFT : GridLocation.RIGHT);
            }
            else
            {
                PlaceFlush(spec.GenDeploy, neg ? GridLocation.BOTTOM : GridLocation.TOP);
            }
        }
        // Diagonal door
        else if (spec.Grid.AlternatesCorners(spec.X, spec.Y, Draw.Walkable<GenSpace>(), out walkableDir))
        {
            spec.GenDeploy.Rotation = walkableDir == GridDirection.DIAGTLBR ? -45 : 45;
        }
        // Offset alternates
        else if (spec.Grid.AlternateSidesOffset(spec.X, spec.Y, Draw.Not(Draw.Walkable<GenSpace>()), out offsetLocation))
        {
            PlaceFlush(spec.GenDeploy, offsetLocation);
        }
    }
}

