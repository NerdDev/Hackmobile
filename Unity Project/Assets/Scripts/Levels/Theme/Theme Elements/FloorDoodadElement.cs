using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class FloorDoodadElement : ThemeElement
{
    public override void PreDeployTweaks(ThemeElementSpec spec)
    {
        CenterAndRotateDoodad(spec);
        PlaceFloors(spec);
    }
}

