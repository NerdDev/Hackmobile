using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BFSSearcher : GridSearcher
{

    public BFSSearcher()
        : base()
    {
    }

    public BFSSearcher(RandomGen rand)
        : base(rand)
    {
    }

    public Array2D<bool> SearchFill(Value2D<GridType> startPoint, GridArray grids, GridSet targets)
    {
        return SearchFill(startPoint, grids, null, targets);
    }

    public Array2D<bool> SearchFill(Value2D<GridType> startPoint, GridArray grids, DrawEval<GridType> pass, GridSet targets)
    {
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Breadth First Search Fill");
            GridArray tmp = new GridArray(grids);
            tmp[startPoint.x, startPoint.y] = GridType.INTERNAL_RESERVED_CUR;
            tmp.ToLog(Logs.LevelGen, "Starting Map:");
        }
        #endregion
        Queue<Value2D<GridType>> queue = new Queue<Value2D<GridType>>();
        queue.Enqueue(startPoint);
        GridType[,] targetArr = grids.GetArr();
        Array2D<bool> outGridArr = new Array2D<bool>(grids.GetBoundingInternal(), false);
        outGridArr[startPoint.x, startPoint.y] = true;
        Value2D<GridType> curPoint;
        DrawActions<GridType> test = DrawPresets.NotEdgeOfArray<GridType>() 
            + new DrawActions<GridType>()
            {
                UnitAction = (arr, x, y) =>
                    {
                        return targets.Contains(arr[y, x]) && !outGridArr[x, y];
                    }
            }
            + (DrawActions<GridType>)pass;
        while (queue.Count > 0)
        {
            curPoint = queue.Dequeue();
            #region DEBUG
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
            {
                outGridArr.ToLog(Logs.LevelGen, "Current Map. Evaluating " + curPoint);
            }
            #endregion
            foreach (Value2D<GridType> option in targetArr.GetAllAround(curPoint.x, curPoint.y, false, test))
            {
                queue.Enqueue(option);
                outGridArr[option.x, option.y] = true;
            }
        }
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen);
        }
        #endregion
        return outGridArr;
    }

}
