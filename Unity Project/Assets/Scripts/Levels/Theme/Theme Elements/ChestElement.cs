using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ChestElement : ThemeElement
{
    const float _chestBuffer = .05F;
    public override void PreDeployTweaks(ThemeElementSpec spec)
    {
        GridLocation wall;
        if (spec.GenGrid.GetRandomLocationAround(spec.DeployX, spec.DeployY, false, spec.Random, Draw.WallType<GenSpace>(), out wall))
        { // If wall around, make it flush
            PlaceFlush(spec.GenDeploy, wall, _chestBuffer);
        }
        else
        { // Place randomly in the middle
            PlaceRandomlyInside(spec.Random, spec.GenDeploy, _chestBuffer);
        }
        PlaceFloors(spec);
    }
}

