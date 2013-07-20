using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class PathTree
{
    public PathNode root;
    public int xdest;
    public int ydest;

    public PathTree()
    {
    }

    public PathTree(int x, int y)
    {
        root = new PathNode(x, y, 0);
        root.parent = null;
    }

    public void setDest(int x, int y)
    {
        this.xdest = x;
        this.ydest = y;
    }

    public bool setPath(PathNode pn)
    {
        //check the 8 potential locations
        //horizontal/vertical
        if (pn.x == xdest && pn.y == ydest)
        {
            pn.dest = true;
            return true;
        }

        if (LevelManager.Array[pn.x, pn.y + 1] == GridType.Floor)
        {
            PathNode pathnew = new PathNode(pn.x, pn.y + 1, pn.d + 1);
            pathnew.parent = pn;
            pn.next.Add(pathnew);
            setPath(pathnew);
        }

        if (LevelManager.Array[pn.x + 1, pn.y] == GridType.Floor)
        {
            PathNode pathnew = new PathNode(pn.x + 1, pn.y, pn.d + 1);
            pathnew.parent = pn;
            pn.next.Add(pathnew);
            setPath(pathnew);
        }

        if (LevelManager.Array[pn.x, pn.y - 1] == GridType.Floor)
        {
            PathNode pathnew = new PathNode(pn.x, pn.y - 1, pn.d + 1);
            pathnew.parent = pn;
            pn.next.Add(pathnew);
            setPath(pathnew);
        }

        if (LevelManager.Array[pn.x - 1, pn.y] == GridType.Floor)
        {
            PathNode pathnew = new PathNode(pn.x - 1, pn.y, pn.d + 1);
            pathnew.parent = pn;
            pn.next.Add(pathnew);
            setPath(pathnew);
        }

        //diagonals
        if (LevelManager.Array[pn.x + 1, pn.y + 1] == GridType.Floor)
        {
            PathNode pathnew = new PathNode(pn.x + 1, pn.y + 1, pn.d + 1);
            pathnew.parent = pn;
            pn.next.Add(pathnew);
            setPath(pathnew);
        }

        if (LevelManager.Array[pn.x - 1, pn.y - 1] == GridType.Floor)
        {
            PathNode pathnew = new PathNode(pn.x - 1, pn.y - 1, pn.d + 1);
            pathnew.parent = pn;
            pn.next.Add(pathnew);
            setPath(pathnew);
        }

        if (LevelManager.Array[pn.x + 1, pn.y - 1] == GridType.Floor)
        {
            PathNode pathnew = new PathNode(pn.x + 1, pn.y - 1, pn.d + 1);
            pathnew.parent = pn;
            pn.next.Add(pathnew);
            setPath(pathnew);
        }

        if (LevelManager.Array[pn.x - 1, pn.y + 1] == GridType.Floor)
        {
            PathNode pathnew = new PathNode(pn.x - 1, pn.y + 1, pn.d + 1);
            pathnew.parent = pn;
            pn.next.Add(pathnew);
            setPath(pathnew);
        }

        return false;
    }
}

public class PathNode
{
    public int x;
    public int y;
    public int d;
    public List<PathNode> next = new List<PathNode>();
    public PathNode parent;
    public bool dest;

    public PathNode(int x, int y, int d)
    {
        this.x = x;
        this.y = y;
        this.d = d;
    }
}

public class AStarPath
{
    Point start, dest;
    public List<AStarNode> path = new List<AStarNode>();

    public AStarPath(Point start, Point dest)
    {
        this.start = start;
        this.dest = dest;
    }

    public void getPath()
    {
        AStarNode startNode = new AStarNode();
        startNode.loc = start;
        startNode.g = 0;
        startNode.h = getManhattan(start, dest);
        startNode.f = startNode.g + startNode.h;

        getNextNodes(startNode);
    }

    public void getNextNodes(AStarNode origin)
    {
        List<AStarNode> nextNodes = new List<AStarNode>();

        //horizontal
        Point nextPoint = new Point(origin.loc.x + 1, origin.loc.y);
        if (LevelManager.Array[nextPoint] == GridType.Floor)
        {
            int h = getManhattan(nextPoint, dest);
            int g = origin.g + 10;
            int f = h + g;
            AStarNode nextNode = new AStarNode();
        }

    }

    public int getManhattan(Point start, Point dest)
    {
        return 10 * (Math.Abs(start.x - dest.x) + Math.Abs(start.y - dest.y));
    }
}

public class AStarNode
{
    public int f;
    public int g;
    public int h;

    public Point loc;
    public Point parent;
}