using UnityEngine;
using System.Collections;

public class PillarMod : RoomModifier
{
    public override bool Unique { get { return true; } }
    static ProbabilityList<int> spacingOptions = new ProbabilityList<int>(Probability.LevelRand);
    static PillarMod()
    {
        spacingOptions.Add(2, 4, false);
        spacingOptions.Add(3, 1, false);
        spacingOptions.Add(4, 2, false);
        spacingOptions.Add(5, 2, false);
    }
    const int differingSpacingChance = 15;

    public override bool Modify(RoomSpec spec)
    {
        Bounding bounds = spec.Room.GetBounding(false);
        int spacingX = spacingOptions.Get();
        int spacingY = Probability.LevelRand.Percent(differingSpacingChance) ? spacingOptions.Get() : spacingX;
        GridType[,] arr = spec.Room.GetArray().GetArr();
        for (int x = bounds.XMin; x < bounds.XMax; x = x + spacingX)
        {
            for (int y = bounds.YMin; y < bounds.YMax; y = y + spacingY)
            {
                if (GridTypeEnum.Walkable(arr[y, x]))
                {
                    if (!arr.Alternates(x, y, GridTypeEnum.Walkable))
                    { // If not blocking a path
                        arr[y, x] = GridType.Wall;
                    }
                }
            }
        }

        return true;
    }

    public override RoomModType GetType()
    {
        return RoomModType.Flexible;
    }

    public override string GetName()
    {
        return "Pillars";
    }
}
