using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RectangularRoom : BaseRoomMod
{
    const int minRectSize = 8;
    const int maxRectSize = 15;
    private int _minSize;
    private int _maxSize;

    public RectangularRoom ()
        : this(minRectSize, maxRectSize)
    {
    }

    public RectangularRoom(int min, int max)
    {
        this._minSize = min;
        this._maxSize = max;
    }

    protected override bool ModifyInternal(RoomSpec spec, double scale)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, ToString());
        }
        #endregion
        int height = spec.Random.Next((int)(_minSize * scale), (int)(_maxSize * scale));
        int width = spec.Random.Next((int)(_minSize * scale), (int)(_maxSize * scale));
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Height: " + height + ", Width: " + width);
        }
        #endregion
        Point center = spec.Grids.Center;
        int left = center.x - (width / 2);
        int bottom = center.y - (height / 2);
        spec.Grids.DrawRect(left, left + width, bottom, bottom + height, new StrokedAction<GenSpace>()
            {
                UnitAction = Draw.SetTo(new GenSpace(GridType.Floor, spec.Theme)),
                StrokeAction = Draw.SetTo(new GenSpace(GridType.Wall, spec.Theme))
            });
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, ToString());
        }
        #endregion
        return true;
    }
}
