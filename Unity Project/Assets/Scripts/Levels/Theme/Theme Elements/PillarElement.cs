using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PillarElement : ThemeElement
{
    public override List<GenDeploy> PreDeployTweaks(ThemeElementSpec spec)
    {
        GenDeploy floorDeploy = new GenDeploy(spec.Theme.Floor.Random(spec.Random));
        return new List<GenDeploy>(new[] { floorDeploy });
    }
}
