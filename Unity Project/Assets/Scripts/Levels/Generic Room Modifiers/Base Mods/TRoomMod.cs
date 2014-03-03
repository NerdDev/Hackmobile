using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TRoomMod : BaseRoomMod
{
    const int shortMin = 4;
    const int shortMax = 7;
    const int longMin = 10;
    const int longMax = 16;
    public override bool Modify(RoomSpec spec)
    {
        // First Rect
        int shortSide = spec.Random.Next(shortMin, shortMax);
        int longSide = spec.Random.Next(longMin, longMax);
        spec.Grids.DrawRect(0, shortSide, 0, longSide, new StrokedAction<GenSpace>()
            {
                UnitAction = Draw.SetTo(GridType.Floor, spec.Theme),
                StrokeAction = Draw.SetTo(GridType.Wall, spec.Theme)
            });

        // Second rect
        int otherLongSide = spec.Random.Next(shortMin, longMax - shortSide);
        int otherShortSide = spec.Random.Next(shortMin, Math.Min(longSide - 2, shortMax));
        bool center = spec.Random.NextBool();
        int yStart;
        if (center)
        {
            yStart = ((longSide / 2) - (otherShortSide / 2));
        }
        else
        {
            int buffer = longSide - otherShortSide / 3;
            int max = longSide - otherShortSide + 1 - buffer;
            if (buffer >= max)
            {
                yStart = ((longSide / 2) - (otherShortSide / 2));
            }
            else
            {
                yStart = spec.Random.Next(buffer, max);
            }
        }
        spec.Grids.DrawRect(shortSide - 1, shortSide + otherLongSide - 1, yStart, yStart + otherShortSide, new StrokedAction<GenSpace>()
           {
               UnitAction = Draw.SetTo(GridType.Floor, spec.Theme),
               StrokeAction = Draw.IsTypeThen(GridType.NULL, Draw.SetTo(GridType.Wall, spec.Theme))
           });

        spec.Grids.Rotate(spec.Random.NextRotation());
        return true;
    }

    public override List<ProbabilityItem<RoomModifier>> GetChainedModifiers()
    {
        return new List<ProbabilityItem<RoomModifier>>(0);
    }
}

