using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class HorizSplitterMod : RoomModifier
{
    protected DrawAction<GridType> ViableSplitter()
    {
        return new DrawAction<GridType>((arr, x, y) =>
            {
                // Not a hallway, and no doors nearby
                return !arr.Alternates(x, y, GridTypeEnum.HallwaySpace) && !arr.DrawAround(x, y, false, DrawPresets.EqualTo(GridType.Door));
            }
            );
    }

    public override bool Modify(RoomSpec spec)
    {
        Bounding bounds = spec.Room.GetBounding(false);
        List<int> options = new List<int>();
        bool horizontal = Probability.LevelRand.NextBool();
        int from = bounds.GetMin(horizontal);
        int to = bounds.GetMax(horizontal);
        int fromAlt = bounds.GetMin(!horizontal);
        int toAlt = bounds.GetMax(!horizontal);

        // Iterate and find all viable options
        for (int i = fromAlt; i < toAlt; i++)
        {
            if (spec.Room.Array.DrawLine(from, to, i, horizontal, ViableSplitter()))
                options.Add(i);
        }
        if (options.Count == 0) return false;
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            foreach (int i in options)
            {
                GridArray copy = new GridArray(spec.Room.GetArray());
                copy.GetArr().DrawLine(from, to, i, horizontal, SetToIfNotNull(GridType.Wall));
                copy.ToLog(Logs.LevelGen);
            }
        }
        #endregion

        // Draw selected splitter
        int picked = options.Random(spec.Random);
        spec.Room.Array.DrawLine(from, to, picked, horizontal, SetToIfNotNull(GridType.Wall));
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Picked splitter:");
            spec.Room.ToLog(Logs.LevelGen);
        }
        #endregion

        // Draw at least one door
        List<Point> doorOptions = new List<Point>();
        spec.Room.Array.DrawLine(from, to, picked, horizontal, LoadDoorOptions(doorOptions));

        int numDoors = Probability.LevelRand.Next(1, 4);
        while (doorOptions.Count > 0 && numDoors > 0)
        {
            Point p = doorOptions.RandomTake(Probability.LevelRand);
            if (spec.Room.Array.CanDrawDoor(p.x, p.y))
            {
                spec.Room.Array[p.y, p.x] = GridType.Door;
                numDoors--;
            }
        }
        return true;
    }

    public override RoomModType GetType()
    {
        return RoomModType.Flexible;
    }

    public override string GetName()
    {
        return "Splitter";
    }
}
