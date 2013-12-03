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
                return !arr.Alternates(x, y, GridTypeEnum.Walkable) && !arr.DrawAround(x, y, false, Draw.EqualTo(GridType.Door));
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
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGenFineSteps))
        {
            foreach (int i in options)
            {
                GridArray copy = new GridArray(spec.Room.GetArray());
                copy.GetArr().DrawLine(from, to, i, horizontal, Draw.SetToIfNotEqual(GridType.NULL, GridType.Wall));
                copy.ToLog(Logs.LevelGen);
            }
        }
        #endregion

        // Draw selected splitter
        int picked = options.Random(spec.Random);
        spec.Room.Array.DrawLine(from, to, picked, horizontal, Draw.SetToIfNotEqual(GridType.NULL, GridType.Wall));
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Picked splitter:");
            spec.Room.ToLog(Logs.LevelGen);
        }
        #endregion

        // Draw at least one door
        GridMap doorOptions = new GridMap();
        spec.Room.Array.DrawLine(from, to, picked, horizontal, 
            Draw.CanDrawDoor().Then(Draw.AddTo(doorOptions)));
        
        int numDoors = Probability.LevelRand.Next(1, 4);
        List<Value2D<GridType>> doors = doorOptions.GetRandomApart(numDoors, LevelGenerator.minDoorSpacing);
        foreach (Value2D<GridType> door in doors)
            spec.Room.Array[door.y, door.x] = GridType.Door;
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
