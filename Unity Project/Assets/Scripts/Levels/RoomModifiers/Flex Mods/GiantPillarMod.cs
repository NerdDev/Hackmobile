using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GiantPillarMod : RoomModifier
{
    public override bool Modify(RoomSpec spec)
    {
        // Add an extra 2 for stroke width for analysis
        int size = spec.Random.Next(2, 5) + 2;
        List<Bounding> locations = spec.Room.GetArray().GetArr().GetSquares(size, size, false, new OptionTests<GridType>()
            {
                UnitAction = (arr, x, y) =>
                    {
                        return arr[y, x] == GridType.Floor;
                    }
                ,
                StrokeAction = (arr, x, y) =>
                    {
                        return GridTypeEnum.Walkable(arr[y, x]);
                    }
            },
            spec.Room.GetBounding(false));
        if (locations.Count == 0) return false;
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Options: ");
            foreach (Bounding r in locations)
            {
                GridArray copy = new GridArray(spec.Room.GetArray());
                copy.GetArr().DrawSquare(r.XMin + 1, r.XMax - 1, r.YMin + 1, r.YMax - 1, Draw.SetTo(GridType.Path_Vert));
                copy.ToLog(Logs.LevelGen);
            }
        }
        #endregion
        Bounding l = locations.Random(Probability.LevelRand);
        // Draw inner square without stroke (stroke was just used to analyze surroundings)
        spec.Room.GetArray().GetArr().DrawSquare(l.XMin + 1, l.XMax - 1, l.YMin + 1, l.YMax - 1, Draw.SetTo(GridType.Wall));
        return true;
    }

    public override RoomModType GetType()
    {
        return RoomModType.Flexible;
    }

    public override string GetName()
    {
        return "Giant Pillar";
    }
}
