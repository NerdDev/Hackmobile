using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DFSSearcher : GridSearcher
{
    Array2D<bool> blockedPoints;
    GridSet targets;

    public DFSSearcher()
        : base()
    {
        Filter = DFSFilter;
    }

    public DFSSearcher(System.Random rand)
        : base(rand)
    {
        Filter = DFSFilter;
    }

    bool DFSFilter(Value2D<GridType> dir)
    {
        return dir != null
            && !blockedPoints[dir.x, dir.y] // If not blocked
            && (dir.val == GridType.NULL || targets.Contains(dir.val)); // If open space or target
    }

    public Stack<Value2D<GridType>> Search(Value2D<GridType> startPoint, GridType[,] arr, GridSet validSpaces, GridSet targets)
    {
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(DebugManager.Logs.LevelGen, "Depth First Search");
            GridArray tmp = new GridArray(arr);
            tmp[startPoint.x, startPoint.y] = GridType.INTERNAL_RESERVED_CUR;
            tmp.ToLog(DebugManager.Logs.LevelGen, "Starting Map:");
        }
        #endregion
        this.targets = targets;
        blockedPoints = new Array2D<bool>(arr.GetLength(1), arr.GetLength(0));
        Stack<Value2D<GridType>> pathTaken = new Stack<Value2D<GridType>>();
        Surrounding<GridType> options = new Surrounding<GridType>(arr);
        options.Filter = Filter;
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
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
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
            options.Load(startPoint.x, startPoint.y);
            #region DEBUG
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
            {
                debugGrid.ToLog(DebugManager.Logs.LevelGen, "Current Map with " + options.Count + " options.");
            }
            #endregion

            // If found target, return path we took
            Value2D<GridType> targetDir = options.GetDirWithVal(true, targets);
            if (targetDir != null)
            {
                #region DEBUG
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
                {
                    BigBoss.Debug.w(DebugManager.Logs.LevelGen, "===== FOUND TARGET: " + startPoint);
                    BigBoss.Debug.printFooter(DebugManager.Logs.LevelGen);
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
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
                {
                    BigBoss.Debug.w(DebugManager.Logs.LevelGen, "Chose Direction: " + targetDir);
                }
                #endregion
                startPoint = targetDir;
                pathTaken.Push(startPoint);
            }
        }
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
        return pathTaken;
    }
}
