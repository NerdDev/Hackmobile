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
    Container2D<bool> blockedPoints;
    Container2D<bool> checkedPoints;
    Container2D<int> jumpTracker;
    List<Value2D<T>> lastJump;
    Value2D<T> targetDir;
    DrawAction<T> allowedSpace;
    DrawAction<T> foundTarget;
    System.Random rand;
    Point gravityPt;

    public JumpTowardsSearcher(Container2D<T> cont, int x, int y, int minJump, int maxJump,
        DrawAction<T> allowedSpace,
        DrawAction<T> target,
        System.Random rand,
        Point gravityPt,
        bool edgeSafe = false)
    {
        this.minJump = minJump;
        this.maxJump = maxJump + 1;
        blockedPoints = Container2D<bool>.CreateArrayFromBounds<T>(cont);
        checkedPoints = Container2D<bool>.CreateArrayFromBounds<T>(cont);
        jumpTracker = Container2D<int>.CreateArrayFromBounds<T>(cont);
        this.container = cont;
        curPoint = new Point(x, y);
        this.allowedSpace = allowedSpace;
        this.rand = rand;
        this.gravityPt = gravityPt;
        this.allowedSpace = (arr2, x2, y2) =>
        {
            return !blockedPoints[x2, y2] && allowedSpace(arr2, x2, y2);
        };
        this.foundTarget = (arr2, x2, y2) =>
        {
            if (!checkedPoints[x2, y2])
            {
                checkedPoints[x2, y2] = true;
                return target(arr2, x2, y2);
            }
            return false;
        };
        if (edgeSafe && this is Array2DRaw<T>)
        {
            this.allowedSpace = this.allowedSpace.And(Draw.NotEdgeOfArray<T>());
            this.foundTarget = this.foundTarget.And(Draw.NotEdgeOfArray<T>());
        }
    }

    public Stack<List<Value2D<T>>> Find()
    {
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen) && typeof(T) == typeof(GridType))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Jump Towards Search");
            BigBoss.Debug.w(Logs.LevelGen, "Starting from (" + curPoint.x + "," + curPoint.y + ")");
            Array2D<GridType> tmpArr = new Array2D<GridType>(container.Bounding);
            foreach (Value2D<T> val in container)
                tmpArr[val] = (GridType)(object)container[val];
            tmpArr[curPoint.x, curPoint.y] = GridType.INTERNAL_RESERVED_CUR;
            tmpArr.ToLog(Logs.LevelGen, "Starting Map:");
        }
        #endregion
        var pathTaken = new Stack<List<Value2D<T>>>();

        // Push start point onto path
        pathTaken.Push(new List<Value2D<T>>(new Value2D<T>[] { new Value2D<T>(curPoint.x, curPoint.y, container[curPoint]) }));
        blockedPoints[curPoint] = true;
        checkedPoints[curPoint] = true;
        while (pathTaken.Count > 0)
        {
            lastJump = pathTaken.Peek();
            curPoint = lastJump[lastJump.Count - 1];

            // Didn't find target, go towards target
            int jump = rand.Next(Math.Max(jumpTracker[curPoint], minJump), maxJump);
            jumpTracker[curPoint] = jump;
            Point dirTowards = gravityPt - curPoint;
            List<Value2D<T>> jumpList;
            if (GetJumpTowards(dirTowards, jump, out jumpList))
            { // Found target
                pathTaken.Push(jumpList);
                return pathTaken;
            }
            if (jumpList == null)
            { // Best direction failed
                List<Point> dirs = Point.Directions;
                dirs.Remove(dirTowards);
                while (dirs.Count > 0)
                {
                    dirTowards = dirs.RandomTake(rand);
                    if (GetJumpTowards(dirTowards, jump, out jumpList))
                    { // Found target
                        pathTaken.Push(jumpList);
                        return pathTaken;
                    }
                    if (jumpList != null)
                    {
                        break;
                    }
                }
                if (jumpList == null)
                { // None found.  Pop
                    foreach (Value2D<T> val in lastJump)
                    {
                        blockedPoints[val] = false;
                    }
                    pathTaken.Pop();
                }
            }
            // Chose a dir
            #region DEBUG
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "Chose Direction: " + dirTowards + " jumping " + jump);
            }
            #endregion
            pathTaken.Push(jumpList);
        }
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Depth First Search");
        }
        #endregion
        return pathTaken;
    }

    protected bool GetJumpTowards(Point dir, int amount, out List<Value2D<T>> ret)
    {
        if (jumpTracker[curPoint + (dir * amount)] >= maxJump)
        { // Already fully tested this route
            ret = null;
            return false;
        }
        // Can test this route
        ret = new List<Value2D<T>>(amount);
        Point cur = new Point(curPoint);
        for (int i = 0; i < amount; i++)
        {
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

            if (allowedSpace(container, cur.x, cur.y))
            { // Jumping
                ret.Add(new Value2D<T>(cur.x, cur.y, container[cur]));
                blockedPoints[cur] = true;
            }
            else
            { // Hit fail
                break;
            }
        }
        if (ret.Count == 0)
            ret = null;
        return false;
    }
}

