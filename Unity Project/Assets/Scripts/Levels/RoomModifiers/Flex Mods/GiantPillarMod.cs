using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GiantPillarMod : RoomModifier
{

    public override bool Modify(RoomSpec spec)
    {
        int size = spec.Random.Next(2, 5);
        List<Bounding> locations = spec.Room.Array.GetSquares(size, size, false, new DrawTest<GridType>()
            {
                UnitTest = new Func<GridType[,], int, int, bool>((arr, x, y) =>
                    {
                        return arr[y, x] == GridType.Floor;
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
                copy.GetArr().DrawSquare(r.XMin, r.XMax, r.YMin, r.YMax, GridType.Path_Vert);
                copy.ToLog(Logs.LevelGen);
            }
        }
        #endregion
        Bounding l = locations.Random(Probability.LevelRand);
        spec.Room.Array.DrawSquare(l.XMin, l.XMax, l.YMin, l.YMax, GridType.Wall);
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
