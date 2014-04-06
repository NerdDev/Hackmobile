using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class WallElement : ThemeElement
{
    public bool PlaceFloor = true;
    public WallElement()
    {
        SetChar('#');
    }
    public override void PreDeployTweaks(ThemeElementSpec spec)
    {
        if (PlaceFloor)
        {
            PlaceFloors(spec);
        }
    }
}

