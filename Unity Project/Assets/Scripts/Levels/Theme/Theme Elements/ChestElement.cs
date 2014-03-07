using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ChestElement : ThemeElement
{
    const float _chestBuffer = .05F;
    public override void PreDeployTweaks(ThemeElementSpec spec)
    {
        spec.Space.Deploys.Add(new GridDeploy(spec.Theme.Get(GridType.Floor, spec.Random).GO));
        GridLocation wall;
        if (spec.Grid.GetRandomLocationAround(spec.X, spec.Y, false, spec.Random, Draw.WallType<GenSpace>(), out wall))
        { // If wall around, make it flush
            PlaceFlush(spec.GenDeploy, wall, _chestBuffer);
        }
        else
        { // Place randomly in the middle
            PlaceRandomlyInside(spec.Random, spec.GenDeploy, _chestBuffer);
        }
    }
}

