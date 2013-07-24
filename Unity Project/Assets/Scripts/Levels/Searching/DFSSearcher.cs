using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DFSSearcher
{
    private System.Random _rand;
    public DFSSearcher()
        : this(new System.Random())
    {
    }

    public DFSSearcher(System.Random rand)
    {
        this._rand = rand;
    }

    public Stack<Value2D<GridType>> Search(Value2D<GridType> startPoint, GridType[,] arr, GridSet validSpaces, GridSet targets)
    {
        #region DEBUG
        if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Depth First Search");
            GridArray tmp = new GridArray(arr);
            tmp[startPoint.x, startPoint.y] = GridType.INTERNAL_RESERVED_CUR;
            tmp.ToLog(DebugManager.Logs.LevelGen, "Starting Map:");
        }
        #endregion
        Array2D<bool> blockedPoints = new Array2D<bool>(arr.GetLength(1), arr.GetLength(0));
        Stack<Value2D<GridType>> pathTaken = new Stack<Value2D<GridType>>();
        FilterDFS filter = new FilterDFS(blockedPoints, targets);
        #region DEBUG
        GridArray debugGrid = new GridArray(0, 0); // Will be reassigned later
        #endregion

        // Push start point onto path
        pathTaken.Push(startPoint);
        while (pathTaken.Count > 0)
        {
            startPoint = pathTaken.Peek();
            // Don't want to visit the same point on a different route later
            blockedPoints[startPoint.x, startPoint.y] = true;
            #region DEBUG
            if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
            { // Set up new print array
                debugGrid = new GridArray(arr);
                // Fill in blocked points
                foreach (Value2D<bool> blockedPt in blockedPoints)
                {
                    if (blockedPt.val)
                    {
                        debugGrid[blockedPt.x, blockedPt.y] = GridType.INTERNAL_RESERVED_BLOCKED;
                    }
                }
                Path tmpPath = new Path(pathTaken);
                debugGrid.PutAll(tmpPath.GetArray());
            }
            #endregion

            // Get surrounding points
            Surrounding<GridType> options = Surrounding<GridType>.Get(arr, startPoint.x, startPoint.y, filter);
            #region DEBUG
            if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                debugGrid.ToLog(DebugManager.Logs.LevelGen, "Current Map with " + options.Count + " options.");
            }
            #endregion

            // If found target, return path we took
            Value2D<GridType> targetDir = options.GetDirWithVal(targets);
            if (targetDir != null)
            {
                #region DEBUG
                if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    DebugManager.w(DebugManager.Logs.LevelGen, "===== FOUND TARGET: " + startPoint);
                    DebugManager.printFooter(DebugManager.Logs.LevelGen);
                }
                #endregion
                pathTaken.Push(targetDir);
                return pathTaken;
            }

            // Didn't find target, pick random direction
            targetDir = options.GetRandom(_rand);
            if (targetDir == null)
            { // If all directions are bad, back up
                pathTaken.Pop();
            }
            else
            {
                #region DEBUG
                if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    DebugManager.w(DebugManager.Logs.LevelGen, "Chose Direction: " + targetDir);
                }
                #endregion
                startPoint = targetDir;
                pathTaken.Push(startPoint);
            }
        }
        #region DEBUG
        if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
        return pathTaken;
    }
}
