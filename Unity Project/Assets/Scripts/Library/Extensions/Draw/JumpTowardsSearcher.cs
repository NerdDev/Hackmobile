﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class JumpTowardsSearcher<T>
{
    Container2D<T> container;
    Point curPoint;
    int minJump;
    int maxJump;
    List<int> jumpAmountOptionPrototype;
    List<int> jumpSmallerOptionPrototype;
    Container2D<JumpSetup> jumps;
    List<Value2D<T>> lastJump;
    DrawAction<T> allowedSpace;
    DrawAction<T> foundTarget;
    System.Random rand;
    Point gravityPt;
    bool edgeSafe;

    public JumpTowardsSearcher(Container2D<T> cont, int x, int y, int minJump, int maxJump,
        DrawAction<T> allowedSpace,
        DrawAction<T> target,
        System.Random rand,
        Point gravityPt,
        bool edgeSafe = false)
    {
        jumps = Container2D<JumpSetup>.CreateArrayFromBounds<T>(cont);
        this.container = cont;
        curPoint = new Point(x, y);
        this.allowedSpace = allowedSpace;
        this.rand = rand;
        this.gravityPt = gravityPt;
        this.foundTarget = target;
        this.edgeSafe = edgeSafe;
        jumpAmountOptionPrototype = new List<int>(maxJump - minJump);
        for (int i = minJump; i <= maxJump; i++)
        {
            jumpAmountOptionPrototype.Add(i);
        }
        jumpSmallerOptionPrototype = new List<int>(minJump);
        for (int i = 1; i < minJump; i++)
        {
            jumpSmallerOptionPrototype.Add(i);
        }
    }

    public Stack<List<Value2D<T>>> Find()
    {
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen) && typeof(T) == typeof(GridType))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Jump Towards Search");
            BigBoss.Debug.w(Logs.LevelGen, "Starting from " + curPoint + " Going towards " + gravityPt);
            MultiMap<GridType> tmpArr = new MultiMap<GridType>();
            foreach (Value2D<T> val in container)
                tmpArr[val] = (GridType)(object)container[val];
            tmpArr[curPoint.x, curPoint.y] = GridType.INTERNAL_RESERVED_CUR;
            tmpArr.ToLog(Logs.LevelGen, "Starting Map:");
        }
        #endregion
        var pathTaken = new Stack<List<Value2D<T>>>();

        // Push start point onto path
        pathTaken.Push(new List<Value2D<T>>(new Value2D<T>[] { new Value2D<T>(curPoint.x, curPoint.y, container[curPoint]) }));
        jumps[curPoint] = new JumpSetup(this, curPoint) { Blocked = true };
        while (pathTaken.Count > 0)
        {
            try
            {
                lastJump = pathTaken.Peek();
                curPoint = lastJump[lastJump.Count - 1];
                List<Value2D<T>> jumpList = null;

                // Didn't find target, go towards target
                JumpSetup jumpSetup = jumps[curPoint];
                while (!jumpSetup.Done)
                {
                    if (GetJumpTowards(jumpSetup, out jumpList))
                    { // Found target
                        pathTaken.Push(jumpList);
                        return pathTaken;
                    }
                    if (jumpList != null)
                    { // Jumped
                        // Chose a dir
                        pathTaken.Push(jumpList);
                        break;
                    }
                }
                if (jumpList == null)
                { // None found.  Pop
                    foreach (Value2D<T> val in lastJump)
                    {
                        jumps[val].Blocked = false;
                    }
                    pathTaken.Pop();
                    #region DEBUG
                    if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                    {
                        BigBoss.Debug.w(Logs.LevelGen, "Backing up from " + curPoint);
                        PrintSetup();
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                BigBoss.Debug.w(Logs.LevelGen, ex.ToString());
            }
        }
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Jump Towards Search");
        }
        #endregion
        return pathTaken;
    }

    bool GetJumpTowards(JumpSetup setup, out List<Value2D<T>> ret)
    {
        Jump jump;
        if (!setup.Get(out jump))
        {
            ret = null;
            return false;
        }
        Point endPoint = curPoint + jump.JumpVector;
        if (edgeSafe && !jumps.InRange(endPoint))
        { // Out of range
            #region DEBUG
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "end point out of array range " + endPoint);
            }
            #endregion
            ret = null;
            return false;
        }
        // Can test this route
        ret = new List<Value2D<T>>(jump.Amount);
        Point cur = new Point(curPoint);
        for (int i = 0; i < jump.Amount; i++)
        {
            cur += jump.Dir;
            // If found target, return path we took
            if (foundTarget(container, cur.x, cur.y))
            {
                #region DEBUG
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                {
                    BigBoss.Debug.w(Logs.LevelGen, "===== FOUND TARGET: " + cur);
                    BigBoss.Debug.printFooter(Logs.LevelGen, "Depth First Search");
                }
                #endregion

                ret.Add(new Value2D<T>(cur.x, cur.y, container[cur]));
                return true;
            }

            JumpSetup space = jumps[cur];
            if (space == null)
            {
                space = new JumpSetup(this, cur);
                jumps[cur] = space;
            }
            if (space.Allowed && !space.Blocked)
            { // Jumping
                ret.Add(new Value2D<T>(cur.x, cur.y, container[cur]));
                space.Blocked = true;
            }
            else
            { // Hit fail
                #region DEBUG
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                {
                    BigBoss.Debug.w(Logs.LevelGen, "failed to step past " + cur + " from " + setup.Point + " in dir " + jump.Dir + " jumping " + jump.Amount);
                }
                #endregion
                //  Clear setup of longer jumps

                break;
            }
        }
        if (ret.Count == 0)
        {
            ret = null;
        }
        else
        {
            #region DEBUG
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "Chose Direction: " + jump.Dir + " from " + setup.Point + " jumping " + (cur - curPoint));
                PrintSetup();
            }
            #endregion
        }
        return false;
    }

    class Jump
    {
        public Point JumpVector;
        public Point Dir;
        public int Amount;

        public Jump(Point dir, int amount)
        {
            Dir = dir;
            Amount = amount;
            JumpVector = Dir * Amount;
        }
    }

    class JumpSetup
    {
        public Point Point;
        public bool Blocked = false;
        public bool Allowed;
        public JumpTowardsSearcher<T> Searcher;
        List<Jump> Jumps;
        int jumpPtr;
        public bool Done
        {
            get
            {
                return jumpPtr >= Jumps.Count;
            }
        }

        public JumpSetup(JumpTowardsSearcher<T> searcher, Point curPoint)
        {
            Point = curPoint;
            Allowed = searcher.allowedSpace(searcher.container, curPoint.x, curPoint.y);
            Point rawDir = searcher.gravityPt - searcher.curPoint;
            Point pref = rawDir.UnitDir();
            if (pref.x != 0 && pref.y != 0)
            {
                if (searcher.rand.NextBool())
                    pref.x = 0;
                else
                    pref.y = 0;
            }
            Point secondary;
            if (pref.x != 0)
            {
                secondary = new Point(0, pref.y > 0 ? 1 : -1);
            }
            else
            {
                secondary = new Point(pref.x > 0 ? 1 : -1, 0);
            }
            this.Searcher = searcher;
            List<Point> tmpDirs = Point.Directions;
            tmpDirs.Remove(pref);
            tmpDirs.Remove(secondary);
            List<Point> dirs = new List<Point>(4);
            dirs.Add(pref);
            dirs.Add(secondary);
            dirs.AddRange(tmpDirs.Randomize(searcher.rand));
            Jumps = new List<Jump>(dirs.Count * searcher.jumpAmountOptionPrototype.Count);
            foreach (int i in searcher.jumpAmountOptionPrototype.Randomize(searcher.rand))
            {
                foreach (Point p in dirs)
                {
                    Jumps.Add(new Jump(p, i));
                }
            }
            if (searcher.jumpSmallerOptionPrototype.Count > 0)
            {
                foreach (int i in searcher.jumpSmallerOptionPrototype.Randomize(searcher.rand))
                {
                    foreach (Point p in dirs)
                    {
                        Jumps.Add(new Jump(p, i));
                    }
                }
            }
        }

        public void Reset()
        {
            jumpPtr = 0;
        }

        public bool Get(out Jump jump)
        {
            if (!Done)
            {
                jump = Jumps[jumpPtr++];
                return true;
            }
            jump = null;
            return false;
        }
    }

    protected void PrintSetup()
    {
        if (typeof(T) != typeof(GridType)) return;
        MultiMap<GridType> tmpArr = new MultiMap<GridType>();
        foreach (Value2D<T> val in container)
            tmpArr[val] = (GridType)(object)container[val];
        foreach (Value2D<JumpSetup> setup in jumps)
        {
            if (setup.val.Blocked)
                tmpArr[setup] = GridType.INTERNAL_RESERVED_BLOCKED;
        }
        tmpArr.ToLog(Logs.LevelGen);
    }
}
