using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TombElement : FloorDoodadElement
{
    public TombTopElement[] TombTop;
    public TombElement()
    {
    }

    public override void PreDeployTweaks(ThemeElementSpec spec)
    {
        base.PreDeployTweaks(spec);
        TombTopElement top = TombTop.Random(spec.Random);
        GenDeploy tombTop = new GenDeploy(top);
    }
}

