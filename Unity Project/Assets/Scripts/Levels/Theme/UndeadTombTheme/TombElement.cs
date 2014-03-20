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
        SetChar((char)(240));
    }

    public override void PreDeployTweaks(ThemeElementSpec spec)
    {
        if (spec.DeployX == -5 && spec.DeployY == -2)
        {
            int wer = 23;
            wer++;
        }
        if (spec.DeployX == 3 && spec.DeployY == -2)
        {
            int wer = 23;
            wer++;
        }
        base.PreDeployTweaks(spec);
        TombTopElement top = TombTop.Random(spec.Random);
        GenDeploy tombTop = new GenDeploy(top);
    }
}

