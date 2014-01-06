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
        List<Bounding> locations = spec.Grids.GetSquares(size, size, false, new OptionTests<GridType>()
            {
                UnitAction = (arr, x, y) => arr[x, y] == GridType.Floor,
                StrokeAction = (arr, x, y) => GridTypeEnum.Walkable(arr[x, y])
            },
            spec.Room.GetBounding(false));
        if (locations.Count == 0) return false;
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Options: ");
            MultiMap<GridType> save = new MultiMap<GridType>();
            Array2D<GridType> copy = new Array2D<GridType>(spec.Room.Grids);
            foreach (Bounding r in locations)
            {
                save.Clear();
                copy.DrawSquare(r.XMin + 1, r.XMax - 1, r.YMin + 1, r.YMax - 1, Draw.AddTo(save).And(Draw.SetTo(GridType.Path_Vert)));
                copy.ToLog(Logs.LevelGen);
                copy.PutAll(save);
            }
        }
        #endregion
        Bounding l = locations.Random(spec.Random);
        // Draw inner square without stroke (stroke was just used to analyze surroundings)
        spec.Grids.DrawSquare(l.XMin + 1, l.XMax - 1, l.YMin + 1, l.YMax - 1, Draw.SetTo(GridType.Wall));
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
