using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PathTree
{
    static PathNode[,] Arr = new PathNode[100, 100];
    protected List<PathNode> closed = new List<PathNode>();
    System.Random rand = new System.Random();
    Value2D<GridSpace> start, dest;
    SortedDictionary<PathNode, PathNode> openNodes = new SortedDictionary<PathNode, PathNode>();

    bool pathComplete;

    public PathTree(Value2D<GridSpace> start, Value2D<GridSpace> dest)
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
        foreach (PathNode pn in listONodes)
        {
            Arr[pn.loc.x, pn.loc.y] = null;
        }
        //*/
        return listONodes;
    }

    public void getNextNodes(PathNode origin)
    {
        origin.isOpen = false;
        if (Arr[origin.loc.x, origin.loc.y] == null)
        {
            Arr[origin.loc.x, origin.loc.y] = origin;
        }
        openNodes.Remove(origin);
        closed.Add(origin);
        pointsToNodes(origin);
    }

    private void checkValidNodes()
    {
        PathNode choice = optimalNode(openNodes);
        if (Arr[choice.loc.x, choice.loc.y] == null)
        {
            Arr[choice.loc.x, choice.loc.y] = choice;
        }
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

    public bool isValidSpace(Value2D<GridSpace> p)
    {
        if (p.x > -1 && p.y > -1)
        {
            try
            {
                if (p.Equals(dest))
                {
                    return true;
                }
                if ((p.val.Type == GridType.Floor || p.val.Type == GridType.Door))
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

    public void pointsToNodes(PathNode origin)
    {
        foreach (Value2D<GridSpace> p in LevelManager.Level.getSurroundingSpaces(origin.loc.x, origin.loc.y))
        {
            if (isValidSpace(p))
            {
                if (Arr[p.x, p.y] == null)
                {
                    PathNode asnode = new PathNode(p, dest, origin);
                    Arr[p.x, p.y] = asnode;
                    openNodes.Add(asnode, asnode);
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
}

public class PathNode : IComparable, IEquatable<PathNode>, IEqualityComparer<PathNode>
{
    public int f;
    public int g;
    public int h;

    public bool isOpen;

    public Value2D<GridSpace> loc;
    public PathNode parent;
    public bool diagonalFromParent;

    public PathNode(Value2D<GridSpace> origin, Value2D<GridSpace> destination, PathNode parent)
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

    private int getManhattan(Value2D<GridSpace> start, Value2D<GridSpace> dest)
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
                    if (this.loc.x == o.loc.x
                        && this.loc.y == o.loc.y)
                    {
                        return 0;
                    }
                    else
                    {
                        System.Random rand = new System.Random();
                        int testVal = rand.Next(0, 2);
                        if (testVal == 0)
                        {
                            return -1;
                        }
                        else
                        {
                            return 1;
                        }
                    }
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
        return this.Equals(node);
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
        if (this == node) //check the reference before checking data
        {
            return true;
        }
        if (this.h != node.h)
        {
            return false;
        }
        if (this.loc.x != node.loc.x)
        {
            return false;
        }
        if (this.loc.y != node.loc.y)
        {
            return false;
        }
        if (this.f != node.f)
        {
            return false;
        }
        if (!this.parent.Equals(node.parent))
        {
            return false;
        }
        if (this.isOpen != node.isOpen)
        {
            return false;
        }
        if (this.diagonalFromParent != node.diagonalFromParent)
        {
            return false;
        }
        if (this.g != node.g)
        {
            return false;
        }
        return true;
    }

    bool IEqualityComparer<PathNode>.Equals(PathNode x, PathNode y)
    {
        return x.Equals(y);
    }

    int IEqualityComparer<PathNode>.GetHashCode(PathNode obj)
    {
        return obj.GetHashCode();
    }
}