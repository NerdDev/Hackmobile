using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PillarMod : FillRoomMod
{
    public override bool Unique { get { return true; } }
    static ProbabilityList<int> spacingOptions = new ProbabilityList<int>();
    static PillarMod()
    {
        spacingOptions.Add(2, 4, false);
        spacingOptions.Add(3, 1, false);
        spacingOptions.Add(4, 2, false);
        spacingOptions.Add(5, 2, false);
    }
    const int differingSpacingChance = 15;

    protected override bool ModifyInternal(RoomSpec spec)
    {
        IPillarTheme pillarTheme = spec.Theme as IPillarTheme;
        if (pillarTheme == null) throw new ArgumentException("Theme must be IPillarTheme");
        ThemeElement pillarElement = pillarTheme.GetPillars().Random(spec.Random);
        Bounding bounds = spec.Grids.Bounding;
        int spacingX = spacingOptions.Get(spec.Random);
        int spacingY = spec.Random.Percent(differingSpacingChance) ? spacingOptions.Get(spec.Random) : spacingX;
        Container2D<GenSpace> arr = spec.Grids;
        var pass = Draw.IsType<GenSpace>(GridType.Floor).And(Draw.Not(Draw.Blocking(Draw.Walkable<GenSpace>())));

        for (int x = bounds.XMin; x < bounds.XMax; x = x + spacingX)
        {
            for (int y = bounds.YMin; y < bounds.YMax; y = y + spacingY)
            {
                if (pass(arr, x, y))
                {
                    GenSpace space = new GenSpace(GridType.Wall, spec.Theme);
                    space.AddDeploy(new GenDeploy(pillarElement));
                }
            }
        }

        return true;
    }
}
