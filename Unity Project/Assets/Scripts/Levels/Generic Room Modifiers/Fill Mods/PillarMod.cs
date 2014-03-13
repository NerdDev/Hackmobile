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
        ThemeElement[] pillarCollection = pillarTheme.GetPillars().Random(spec.Random).Elements;
        Bounding bounds = spec.Grids.Bounding;
        int spacingX = spacingOptions.Get(spec.Random);
        int spacingY = spec.Random.Percent(differingSpacingChance) ? spacingOptions.Get(spec.Random) : spacingX;
        Container2D<GenSpace> arr = spec.Grids;
        var call = Draw.IsType<GenSpace>(GridType.Floor).And(Draw.Not(Draw.Blocking(Draw.Walkable<GenSpace>())))
            .IfThen(Draw.MergeIn(pillarCollection, spec.Random, spec.Theme));

        for (int x = bounds.XMin; x < bounds.XMax; x = x + spacingX)
        {
            for (int y = bounds.YMin; y < bounds.YMax; y = y + spacingY)
            {
                call(arr, x, y);
            }
        }

        return true;
    }
}
