using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ClusterElement : ThemeElement
{
    public ThemeElement[] ClusterElements;

    public override void PreDeployTweaks(ThemeElementSpec spec)
    {
        spec.GenDeploy.Element = ClusterElements.Random(spec.Random);
        spec.GenDeploy.Rotate(spec.Random.NextRotation());
        GenDeploy up = new GenDeploy(ClusterElements.Random(spec.Random));
        up.Z = .25F;
        up.Rotate(spec.Random.NextRotation());
        spec.AddAdditional(up);
        GenDeploy down = new GenDeploy(ClusterElements.Random(spec.Random));
        down.Z = -0.25F;
        down.Rotate(spec.Random.NextRotation());
        spec.AddAdditional(down);
        GenDeploy left = new GenDeploy(ClusterElements.Random(spec.Random));
        left.X = -0.25F;
        left.Rotate(spec.Random.NextRotation());
        spec.AddAdditional(left);
        GenDeploy right = new GenDeploy(ClusterElements.Random(spec.Random));
        right.X = 0.25F;
        right.Rotate(spec.Random.NextRotation());
        spec.AddAdditional(right);
        //GridLocationResults results = spec.DeployGrid.DrawLocationsAroundResults(spec.DeployX, spec.DeployY, true,
        //    Draw.ContainsThemeElement(this));
        PlaceFloors(spec);
    }
}

