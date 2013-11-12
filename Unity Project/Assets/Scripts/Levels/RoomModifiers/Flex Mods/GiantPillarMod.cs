using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GiantPillarMod : RoomModifier
{

    public override bool Modify(RoomSpec spec)
    {
        int size = spec.Random.Next(2, 5);
        List<Rectangle> locations = spec.Room.Array.GetSquares(size, size, false, new DrawTest<GridType>()
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
            foreach (Rectangle r in locations)
            {
                BigBoss.Debug.w(Logs.LevelGen, "Options: ");
                GridArray copy = new GridArray(spec.Room.GetArray());
                copy.GetArr().DrawSquare(r.Left, r.Right, r.Bottom, r.Top, GridType.Path_Vert);
                copy.ToLog(Logs.LevelGen);
            }
        }
        #endregion
        Rectangle l = locations.Random(Probability.LevelRand);
        spec.Room.Array.DrawSquare(l.Left, l.Right, l.Bottom, l.Top, GridType.Wall);
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
