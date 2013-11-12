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
        List<Bounding> locations = spec.Room.Array.GetSquares(size, size, false, new SquareTest<GridType>()
            {
                UnitTest = new Func<GridType[,], int, int, bool>((arr, x, y) =>
                    {
                        return arr[y, x] == GridType.Floor;
                    }
                ),
                StrokeTest = new Func<GridType[,], int, int, bool>((arr, x, y) =>
                    {
                        return GridTypeEnum.Walkable(arr[y, x]);
                    }
                )
            });
        if (locations.Count == 0) return false;
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            foreach (Bounding r in locations)
            {
                BigBoss.Debug.w(Logs.LevelGen, "Options: ");
                GridArray copy = new GridArray(spec.Room.GetArray());
                copy.GetArr().DrawSquare(r.XMin + 1, r.XMax - 1, r.YMin + 1, r.YMax - 1, GridType.Path_Vert);
                copy.ToLog(Logs.LevelGen);
            }
        }
        #endregion
        Bounding l = locations.Random(Probability.LevelRand);
        // Draw inner square without stroke (stroke was just used to analyze surroundings)
        spec.Room.Array.DrawSquare(l.XMin + 1, l.XMax - 1, l.YMin + 1, l.YMax - 1, GridType.Wall);
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
