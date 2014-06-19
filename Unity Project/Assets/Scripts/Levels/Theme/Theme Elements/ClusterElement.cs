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
        spec.GenDeploy.YRotation = spec.Random.NextRotationDegree();
        GenDeploy up = new GenDeploy(ClusterElements.Random(spec.Random));
        up.Z = .33F;
        up.YRotation = spec.Random.NextRotationDegree();
        spec.AddAdditional(up);
        GenDeploy down = new GenDeploy(ClusterElements.Random(spec.Random));
        down.Z = -0.33F;
        down.YRotation = spec.Random.NextRotationDegree();
        spec.AddAdditional(down);
        GenDeploy left = new GenDeploy(ClusterElements.Random(spec.Random));
        left.X = -0.33F;
        left.Rotate(spec.Random.NextRotation());
        spec.AddAdditional(left);
        GenDeploy right = new GenDeploy(ClusterElements.Random(spec.Random));
        right.X = 0.33F;
        right.YRotation = spec.Random.NextRotationDegree();
        spec.AddAdditional(right);
        GridLocationResults results = spec.GenGrid.DrawLocationsAroundResults(spec.DeployX, spec.DeployY, true,
            Draw.ContainsThemeElement(this));
        if (results[GridLocation.RIGHT]
            || results[GridLocation.TOP]
            || results[GridLocation.TOPRIGHT])
        {
            GenDeploy topRight = new GenDeploy(ClusterElements.Random(spec.Random));
            topRight.X = 0.33F;
            topRight.Z = 0.33F;
            topRight.YRotation = spec.Random.NextRotationDegree();
            spec.AddAdditional(topRight);
        }
        if (results[GridLocation.RIGHT]
            || results[GridLocation.BOTTOM]
            || results[GridLocation.BOTTOMRIGHT])
        {
            GenDeploy bottomRight = new GenDeploy(ClusterElements.Random(spec.Random));
            bottomRight.X = 0.33F;
            bottomRight.Z = -0.33F;
            bottomRight.YRotation = spec.Random.NextRotationDegree();
            spec.AddAdditional(bottomRight);
        }
        if (results[GridLocation.LEFT]
            || results[GridLocation.BOTTOM]
            || results[GridLocation.BOTTOMLEFT])
        {
            GenDeploy bottomLeft = new GenDeploy(ClusterElements.Random(spec.Random));
            bottomLeft.X = -0.33F;
            bottomLeft.Z = -0.33F;
            bottomLeft.YRotation = spec.Random.NextRotationDegree();
            spec.AddAdditional(bottomLeft);
        }
        if (results[GridLocation.LEFT]
            || results[GridLocation.TOP]
            || results[GridLocation.TOPLEFT])
        {
            GenDeploy topLeft = new GenDeploy(ClusterElements.Random(spec.Random));
            topLeft.X = -0.33F;
            topLeft.Z = 0.33F;
            topLeft.YRotation = spec.Random.NextRotationDegree();
            spec.AddAdditional(topLeft);
        }
    }
}

