using System;
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
    bool hugCorners;
    Stack<List<Value2D<T>>> pathTaken = new Stack<List<Value2D<T>>>();

    public JumpTowardsSearcher(Container2D<T> cont, int x, int y, int minJump, int maxJump,
        DrawAction<T> allowedSpace,
        DrawAction<T> target,
        System.Random rand,
        Point gravityPt,
        bool hugCorners = true,
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
        this.minJump = minJump;
        this.maxJump = maxJump;
        this.hugCorners = hugCorners;
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
        // Push start point onto path
        lastJump = new List<Value2D<T>>(new Value2D<T>[] { new Value2D<T>(curPoint.x, curPoint.y, container[curPoint]) });
        pathTaken.Push(lastJump);
        jumps[curPoint] = new JumpSetup(this, curPoint, curPoint);
        try
        {
            while (pathTaken.Count > 0)
            {
                curPoint = lastJump[lastJump.Count - 1];

                // Didn't find target, go towards target
                JumpSetup jumpSetup = jumps[curPoint];
                if (GetJumpTowards(jumpSetup, out lastJump))
                { // Found target
                    pathTaken.Push(lastJump);
                    break;
                }
                if (lastJump.Count > 0)
                { // Jumped
                    // Chose a dir
                    pathTaken.Push(lastJump);
                    #region DEBUG
                    if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                    {
                        PrintSetup();
                    }
                    #endregion
                }
                else
                { // None found.  Pop
                    if (lastJump.Count <= 1)
                    {
                        pathTaken.Pop();
                    }
                    else
                    {
                        lastJump.RemoveAt(lastJump.Count - 1);
                    }
                    #region DEBUG
                    if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                    {
                        BigBoss.Debug.w(Logs.LevelGen, "Backing up from " + curPoint.x + " " + curPoint.y);
                        PrintSetup();
                    }
                    #endregion
                    if (pathTaken.Count > 0)
                    {
                        lastJump = pathTaken.Peek();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            BigBoss.Debug.w(Logs.LevelGen, ex.ToString());
            #region DEBUG
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.printFooter(Logs.LevelGen, "Jump Towards Search");
            }
            #endregion
            throw;
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
        ret = new List<Value2D<T>>(setup.Amount);
        Point last;
        while (!setup.Done)
        {
            Point dir = setup.Dirs[setup.DirPtr++];
            Point endPoint = curPoint + dir;
            if (edgeSafe && !jumps.InRange(endPoint))
            { // Out of range
                #region DEBUG
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                {
                    BigBoss.Debug.w(Logs.LevelGen, "end point out of array range " + endPoint);
                }
                #endregion
                ret = new List<Value2D<T>>(0);
                return false;
            }
            // Can test this route
            Point cur = new Point(curPoint);
            for (int i = 1; i <= setup.Amount; i++)
            {
                last = cur;
                cur += dir;
                JumpSetup space = jumps[cur];
                if (space == null)
                {
                    space = new JumpSetup(this, cur, last);
                    jumps[cur] = space;
                    if (space.Allowed)
                    {
                        ret.Add(new Value2D<T>(cur.x, cur.y, container[cur]));
                        // If found target, return path we took
                        Value2D<T> found;
                        if (container.GetPointAround(cur.x, cur.y, false, foundTarget, out found))
                        {
                            #region DEBUG
                            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                            {
                                BigBoss.Debug.w(Logs.LevelGen, "===== FOUND TARGET: " + found);
                            }
                            #endregion

                            ret.Add(new Value2D<T>(found.x, found.y, container[found]));
                            return true;
                        }
                        continue;
                    }
                }
                // Blocked
                #region DEBUG
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                {
                    BigBoss.Debug.w(Logs.LevelGen, "failed to step past " + cur + " from " + setup.Point + " in dir " + dir + " jumping " + setup.Amount);
                }
                #endregion
                if (hugCorners)
                {
                    if (ret.Count == 0)
                    {
                        setup.Amount = 1;
                    }
                    else
                    {
                        jumps[cur - dir].Amount = 1;
                    }
                }
                break;
            }
            #region DEBUG
            if (ret.Count > 0)
            {
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                {
                    BigBoss.Debug.w(Logs.LevelGen, "Chose Direction: " + dir + " from " + setup.Point + " jumping " + (cur - curPoint));
                }
                return false;
            }
            #endregion
        }
        ret.Clear();
        return false;
    }

    class JumpSetup
    {
        public Point Point;
        public bool Allowed;
        public JumpTowardsSearcher<T> Searcher;
        public int Amount;
        public List<Point> Dirs;
        public int DirPtr;
        public bool Done
        {
            get
            {
                return DirPtr >= Dirs.Count;
            }
        }

        public JumpSetup(JumpTowardsSearcher<T> searcher, Point curPoint, Point from)
        {
            Amount = searcher.rand.Next(searcher.minJump, searcher.maxJump);
            Point = curPoint;
            Allowed = searcher.allowedSpace(searcher.container, curPoint.x, curPoint.y);
            Point rawDir = searcher.gravityPt - searcher.curPoint;
            this.Searcher = searcher;
            List<Point> tmpDirs = Point.Directions;
            Dirs = new List<Point>(3);
            // Remove from dir
            tmpDirs.Remove(from);
            // Get momentum
            Point momentum = (curPoint - from).UnitDir();

            // Set preferred route
            Point pref = rawDir.UnitDir();
            if (pref.x != 0 && pref.y != 0)
            {
                if (momentum.isZero())
                { // No momentum, so random
                    if (searcher.rand.NextBool())
                        pref.x = 0;
                    else
                        pref.y = 0;
                }
                // Momentum 
                else if (momentum.x != 0)
                {
                    pref.y = 0;
                }
                else
                {
                    pref.x = 0;
                }
            }
            Point secondary = null;
            if (pref.x != 0 && rawDir.y != 0)
            {
                secondary = new Point(0, rawDir.y > 0 ? 1 : -1);
            }
            else if (rawDir.x != 0)
            {
                secondary = new Point(rawDir.x > 0 ? 1 : -1, 0);
            }
            tmpDirs.Remove(pref);
            Dirs.Add(pref);
            // Add secondary route
            if (secondary != null)
            {
                Dirs.Add(secondary);
                tmpDirs.Remove(secondary);
            }
            // Add momentum
            if (tmpDirs.Remove(momentum))
            {
                Dirs.Add(momentum);
            }
            Dirs.AddRange(tmpDirs.Randomize(searcher.rand));
        }

        public void PrintSetup()
        {
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w("Plan at point " + Point);
                foreach (Point dir in Dirs)
                {
                    BigBoss.Debug.w(Logs.LevelGen, dir.ToString());
                }
            }
        }
    }

    protected IEnumerable<Value2D<T>> Enumerate()
    {
        foreach (List<Value2D<T>> list in pathTaken.Reverse())
        {
            foreach (Value2D<T> val in list)
            {
                yield return val;
            }
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
            tmpArr[setup] = GridType.INTERNAL_RESERVED_BLOCKED;
        }
        foreach (Value2D<GridType> val in Enumerate().Cast<Point>())
        {
            tmpArr[val] = GridType.INTERNAL_RESERVED_CUR;
        }
        foreach (Value2D<GridType> val in Path.PathPrint(Enumerate().Cast<Point>()))
        {
            tmpArr[val] = val.val;
        }
        tmpArr.ToLog(Logs.LevelGen);
    }
}

