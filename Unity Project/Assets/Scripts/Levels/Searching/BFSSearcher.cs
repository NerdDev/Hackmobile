using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BFSSearcher : Searcher {
    
    public BFSSearcher()
        : base()
    {
    }

    public BFSSearcher(System.Random rand)
        : base(rand)
    {
    }

    public Array2D<bool> SearchFill(Value2D<GridType> startPoint, GridArray grids, GridSet targets)
    {
        return SearchFill(startPoint, grids, null, targets);
    }

    public Array2D<bool> SearchFill(Value2D<GridType> startPoint, GridArray grids, PassFilter<Value2D<GridType>> pass, GridSet targets)
    {
        #region DEBUG
        if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Breadth First Search Fill");
            GridArray tmp = new GridArray(grids);
            tmp[startPoint.x, startPoint.y] = GridType.INTERNAL_RESERVED_CUR;
            tmp.ToLog(DebugManager.Logs.LevelGen, "Starting Map:");
        }
        #endregion
        Queue<Value2D<GridType>> queue = new Queue<Value2D<GridType>>();
        queue.Enqueue(startPoint);
        GridType[,] targetArr = grids.GetArr();
        Array2D<bool> outGridArr = new Array2D<bool>(grids.GetBoundingInternal(), false);
        Surrounding<GridType> options = new Surrounding<GridType>(targetArr);
        options.Filter = pass;
        outGridArr[startPoint.x, startPoint.y] = true;
        Value2D<GridType> curPoint;
        while (queue.Count > 0)
        {
            curPoint = queue.Dequeue();
            options.Load(curPoint.x, curPoint.y);
            #region DEBUG
            if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                outGridArr.ToLog(DebugManager.Logs.LevelGen, "Current Map with " + options.Count + " options. Evaluating " + curPoint);
            }
            #endregion
            foreach (Value2D<GridType> option in options)
            {
                if (targets.Contains(option.val) && !outGridArr[option.x, option.y])
                {
                    queue.Enqueue(option);
                    outGridArr[option.x, option.y] = true;
                }
            }
        }
        #region DEBUG
        if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
        return outGridArr;
    }

}
