﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class FloorDoodadElement : ThemeElement
{
    public override List<GenDeploy> PreDeployTweaks(ThemeElementSpec spec)
    {
        return PlaceFloors(spec);
    }
}

