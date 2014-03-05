using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TRoomMod : BaseRoomMod
{
    public int ShortMin = 4;
    public int ShortMax = 7;
    public int LongMin = 10;
    public int LongMax = 16;

    public TRoomMod()
    {
    }

    protected override bool ModifyInternal(RoomSpec spec, double scale)
    {
        ShortMax = (int)(ShortMax * scale);
        ShortMin = (int)(ShortMin * scale);
        LongMin = (int)(LongMin * scale);
        LongMax = (int)(LongMax * scale);
        // First Rect
        int shortSide = spec.Random.Next(ShortMin, ShortMax);
        int longSide = spec.Random.Next(LongMin, LongMax);
        spec.Grids.DrawRect(0, shortSide, 0, longSide, new StrokedAction<GenSpace>()
            {
                UnitAction = Draw.SetTo(GridType.Floor, spec.Theme),
                StrokeAction = Draw.SetTo(GridType.Wall, spec.Theme)
            });

        // Second rect
        int otherLongSide = spec.Random.Next(ShortMin, LongMax - shortSide);
        int otherShortSide = spec.Random.Next(ShortMin, Math.Min(longSide - 2, ShortMax));
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
}

