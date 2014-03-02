using UnityEngine;
using System.Collections;

public class PillarMod : FillRoomMod
{
    public override string Name { get { return "Pillars"; } }
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

    public override bool Modify(RoomSpec spec)
    {
        Bounding bounds = spec.Room.GetBounding(false);
        int spacingX = spacingOptions.Get(spec.Random);
        int spacingY = spec.Random.Percent(differingSpacingChance) ? spacingOptions.Get(spec.Random) : spacingX;
        Container2D<GenSpace> arr = spec.Grids;
        for (int x = bounds.XMin; x < bounds.XMax; x = x + spacingX)
        {
            for (int y = bounds.YMin; y < bounds.YMax; y = y + spacingY)
            {
                GenSpace space;
                if (arr.TryGetValue(x, y, out space)
                    && GridTypeEnum.Walkable(space.Type)
                    && !arr.AlternatesSides(x, y, Draw.Walkable<GenSpace>()))
                {
                    { // If not blocking a path
                        arr.SetTo(x, y, new GenSpace(GridType.Pillar, spec.Theme));
                    }
                }
            }
        }

        return true;
    }
}
