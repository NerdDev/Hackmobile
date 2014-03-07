using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PillarElement : ThemeElement
{
    public override void PreDeployTweaks(ThemeElementSpec spec)
    {
        base.PreDeployTweaks(spec);
        spec.Space.Deploys.Add(new GridDeploy(spec.Theme.Floor.Random(spec.Random).GO));
    }
}
