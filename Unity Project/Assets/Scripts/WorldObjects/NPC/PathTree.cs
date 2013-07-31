using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PathTree
{
    static PathNode[,] Arr = new PathNode[1000, 1000];
    protected List<PathNode> closed = new List<PathNode>();
    P start, dest;
    SortedDictionary<PathNode, PathNode> openNodes = new SortedDictionary<PathNode, PathNode>();

    bool pathComplete;

    public PathTree(P start, P dest)
    {
        this.start = start;
        this.dest = dest;
    }

    public List<PathNode> getPath()
    {
        PathNode startNode = new PathNode(start, dest, null);
        startNode.g = 0;
        Arr[start.x, start.y] = startNode;
        getNextNodes(startNode);
        while (!pathComplete && openNodes.Count > 0)
        {
            checkValidNodes();
        }
        ///*
        foreach (PathNode pn in closed)
        {
            Arr[pn.loc.x, pn.loc.y] = null;
        }
        foreach (PathNode pn in openNodes.Keys)
        {
            Arr[pn.loc.x, pn.loc.y] = null;
        }
        //*/
        return listONodes;
    }

    public void getNextNodes(PathNode origin)
    {
        origin.isOpen = false;
        openNodes.Remove(origin);
        closed.Add(origin);
        List<P> surroundingSpaces = getSurroundingSpaces(origin.loc);
        pointsToNodes(surroundingSpaces, origin);
    }

    private void checkValidNodes()
    {
        PathNode choice = optimalNode(openNodes);

        if (choice != null)
        {
            if (choice.loc.x == dest.x && choice.loc.y == dest.y)
            {
                pathComplete = true;
                listONodes.Add(choice);
                buildPath(choice);
                return;
            }
            else
            {
                getNextNodes(choice);
            }
        }
    }

    public List<P> getSurroundingSpaces(P root)
    {
        List<P> nodes = new List<P>();
        P p;

        //horizontal
        p = new P(root.x, root.y + 1);
        if (isValidSpace(p))
        {
            nodes.Add(p);
        }
        p = new P(root.x, root.y - 1);
        if (isValidSpace(p))
        {
            nodes.Add(p);
        }

        //vertical
        p = new P(root.x + 1, root.y);
        if (isValidSpace(p))
        {
            nodes.Add(p);
        }
        p = new P(root.x - 1, root.y);
        if (isValidSpace(p))
        {
            nodes.Add(p);
        }

        //diagonals
        p = new P(root.x + 1, root.y + 1);
        if (isValidSpace(p))
        {
            nodes.Add(p);
        }
        p = new P(root.x - 1, root.y - 1);
        if (isValidSpace(p))
        {
            nodes.Add(p);
        }
        p = new P(root.x + 1, root.y - 1);
        if (isValidSpace(p))
        {
            nodes.Add(p);
        }
        p = new P(root.x - 1, root.y + 1);
        if (isValidSpace(p))
        {
            nodes.Add(p);
        }
        return nodes;
    }

    public bool isValidSpace(P p)
    {
        if (p.x > -1 && p.y > -1)
        {
            try
            {
                if (!LevelManager.Level[p.x, p.y].IsBlocked())
                {
                    return true;
                }
            }
            catch (NullReferenceException)
            {
                //the array location doesn't exist
            }
        }
        return false;
    }

    public void pointsToNodes(List<P> list, PathNode origin)
    {
        foreach (P p in list)
        {
            if (Arr[p.x, p.y] == null)
            {
                PathNode asnode = new PathNode(p, dest, origin);
                try
                {
                    openNodes.Add(asnode, asnode);
                }
                catch
                {

                }
                Arr[p.x, p.y] = asnode;
            }

            else
            {
                PathNode asnode = Arr[p.x, p.y];
                if (asnode.isOpen)
                {
                    int curG = asnode.g;
                    int testG;
                    bool diag = asnode.getDiagonal(origin);

                    if (diag)
                    {
                        testG = origin.g + 10;
                    }
                    else
                    {
                        testG = origin.g + 14;
                    }

                    if (curG > testG)
                    {
                        asnode.setParent(origin, diag);
                    }
                }
            }

        }
    }

    public PathNode optimalNode(SortedDictionary<PathNode, PathNode> list)
    {
        foreach (PathNode node in list.Keys)
        {
            if (node.isOpen)
            {
                return node;
            }
            else
            {
                continue;
            }
        }
        return null;
    }

    private List<PathNode> listONodes = new List<PathNode>();
    private void buildPath(PathNode node)
    {
        if (node.parent != null)
        {
            listONodes.Add(node.parent);
            buildPath(node.parent);
        }
    }

    public PathNode pickNode(List<PathNode> list)
    {
        System.Random rand = new System.Random();
        return list[rand.Next(list.Count)];
    }
}

public class PathNode : IComparable, IEquatable<PathNode>
{
    public int f;
    public int g;
    public int h;

    public bool isOpen;

    public Value2D<int> loca;
    public P loc;
    public PathNode parent;
    public bool diagonalFromParent;

    public PathNode(P origin, P destination, PathNode parent)
    {
        loc = origin;
        h = getManhattan(origin, destination);
        isOpen = true;
        if (parent != null)
        {
            this.parent = parent;
            diagonalFromParent = getDiagonal();
            if (diagonalFromParent)
            {
                g = parent.g + 14;
            }
            else
            {
                g = parent.g + 10;
            }
        }
        else
        {
            this.parent = null;
            this.g = 0;
            this.diagonalFromParent = false;
        }
        f = g + h;
    }

    private int getManhattan(P start, P dest)
    {
        int x = Math.Abs(start.x - dest.x);
        int y = Math.Abs(start.y - dest.y);
        if (x > y)
        {
            return 14 * y + 10 * (x - y);
        }
        else
        {
            return 14 * x + 10 * (y - x);
        }
    }

    protected bool getDiagonal()
    {
        if (Math.Abs(this.loc.x - parent.loc.x) == 1 && Math.Abs(this.loc.y - parent.loc.y) == 1)
        {
            return true;
        }
        else if ((Math.Abs(this.loc.x - parent.loc.x) == 1 && this.loc.y == parent.loc.y)
            || (Math.Abs(this.loc.y - parent.loc.y) == 1 && this.loc.x == parent.loc.x))
        {
            return false;
        }
        return false;
    }

    public bool getDiagonal(PathNode parent)
    {
        if (Math.Abs(this.loc.x - parent.loc.x) == 1 && Math.Abs(this.loc.y - parent.loc.y) == 1)
        {
            return true;
        }
        return false;
    }

    public void setParent(PathNode parent)
    {
        this.parent = parent;
        //checks if diagonal
        bool diagonalFromParent = getDiagonal();
        if (!diagonalFromParent)
        {
            g = parent.g + 10;
        }
        else
        {
            g = parent.g + 14;
        }
        f = g + h;
    }

    public void setParent(PathNode parent, bool diagonal)
    {
        this.parent = parent;
        //checks if diagonal
        bool diagonalFromParent = diagonal;
        if (!diagonalFromParent)
        {
            g = parent.g + 10;
        }
        else
        {
            g = parent.g + 14;
        }
        f = g + h;
    }

    public int CompareTo(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return 1;
        }

        PathNode o = obj as PathNode;
        if (this.f < o.f)
        {
            return -1;
        }
        else if (this.f == o.f)
        {
            if (this.g < o.g)
            {
                return -1;
            }
            else if (this.g == o.g)
            {
                if (this.h < o.h)
                {
                    return -1;
                }
                else if (this.h == o.h)
                {
                    return 0;
                }
            }
        }
        return 1;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        PathNode node = obj as PathNode;
        if (f != node.f)
        {
            return false;
        }
        if (g != node.g)
        {
            return false;
        }
        if (h != node.h)
        {
            return false;
        }
        if (loc.x != node.loc.x && loc.y != node.loc.y)
        {
            return false;
        }
        return true;
    }

    public override int GetHashCode()
    {
        int hash = 7;
        hash = 7 * hash + f;
        hash = 4 * hash + g;
        hash = 3 * hash + h;
        hash = 5 * hash + loc.GetHashCode();
        if (parent != null)
        {
            hash = 9 * hash + parent.GetHashCode();
        }
        else
        {
            hash = 9 * hash + 5;
        }
        return hash;
    }

    public bool Equals(PathNode node)
    {
        if (node == null)
        {
            return false;
        }
        if (this == node) //check the reference before checking data
        {
            return true;
        }
        if (h != node.h)
        {
            return false;
        }
        if (loc.x != node.loc.x)
        {
            return false;
        }
        if (loc.y != node.loc.y)
        {
            return false;
        }
        return true;
    }
}

public class P
{
    public int x;
    public int y;

    public P(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public P(float x, float y)
    {
        this.x = Convert.ToInt32(x);
        this.y = Convert.ToInt32(y);
    }
}