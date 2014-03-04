using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GrandTombRoom : BaseRoomMod
{
    public override bool Modify(RoomSpec spec)
    {
        int numTombs = spec.Random.Next(2, 5) * 2;
        List<Bounding> largestRects = spec.Grids.FindLargestRectangles(false, new StrokedAction<GenSpace>()
        {
            UnitAction = Draw.IsType<GenSpace>(GridType.Floor)
        }, spec.Grids.Bounding);
        if (largestRects.Count == 0) return false;
        #region DEBUG
        //if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
        //{
        //    for (Bounding bounds largestRects)
        //    {

        //    }
        //}
        #endregion
        return false;
    }
}

