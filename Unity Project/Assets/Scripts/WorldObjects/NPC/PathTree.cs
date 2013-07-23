using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PathTree
{
    Point start, dest;
    public List<PathNode> path = new List<PathNode>();
    List<PathNode> openNodes = new List<PathNode>();
    List<PathNode> closedNodes = new List<PathNode>();
    bool pathComplete;

    public PathTree(Point start, Point dest)
    {
        this.start = start;
        this.dest = dest;
    }

    public List<PathNode> getPath()
    {
        PathNode startNode = new PathNode(start, dest, null);
        startNode.g = 0;
        getNextNodes(startNode);
        while (!pathComplete && openNodes.Count > 0)
        {
            checkValidNodes();
        }
        return listONodes;
    }

    public void getNextNodes(PathNode origin)
    {
        closedNodes.Add(origin);
        openNodes.Remove(origin);
        List<Point> surrounding = getSurroundingSpaces(origin.loc);
        surrounding = getValidSpaces(surrounding);
        pointsToNodes(surrounding, origin);
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

    public List<Point> getSurroundingSpaces(Point root)
    {
        List<Point> nodes = new List<Point>();
        //horizontal
        nodes.Add(new Point(root.x, root.y + 1));
        nodes.Add(new Point(root.x, root.y - 1));

        //vertical
        nodes.Add(new Point(root.x + 1, root.y));
        nodes.Add(new Point(root.x - 1, root.y));

        //diagonals
        nodes.Add(new Point(root.x + 1, root.y + 1));
        nodes.Add(new Point(root.x - 1, root.y - 1));
        nodes.Add(new Point(root.x + 1, root.y - 1));
        nodes.Add(new Point(root.x - 1, root.y + 1));
        return nodes;
    }

    public List<Point> getValidSpaces(List<Point> list)
    {
        List<Point> newList = new List<Point>();

        foreach (Point p in list)
        {
            if (LevelManager.Array[p.x, p.y] == GridType.Floor || LevelManager.Array[p.x, p.y] == GridType.Door
                || LevelManager.Array[p.x, p.y] == GridType.Path_LB
                || LevelManager.Array[p.x, p.y] == GridType.Path_LT
                || LevelManager.Array[p.x, p.y] == GridType.Path_RB
                || LevelManager.Array[p.x, p.y] == GridType.Path_RT
                || LevelManager.Array[p.x, p.y] == GridType.Path_Vert
                || LevelManager.Array[p.x, p.y] == GridType.Path_Horiz)
            {
                newList.Add(p);
            }
        }

        return newList;
    }

    public void pointsToNodes(List<Point> list, PathNode origin)
    {
        foreach (Point p in list)
        {
            PathNode asnode = new PathNode(p, dest, origin);
            if (!openNodes.Contains(asnode) && !closedNodes.Contains(asnode))
            {
                openNodes.Add(asnode);
            }
            else if (openNodes.Contains(asnode) && !closedNodes.Contains(asnode))
            {
                asnode = openNodes.Find(
                    delegate(PathNode node)
                    {
                        return (node.loc.x == p.x && node.loc.y == p.y);
                    }
                );
                int curG = asnode.g;
                PathNode priorParent = asnode.parent;
                asnode.setParent(origin);
                int testG = asnode.g;

                if (curG >= testG)
                {
                    //do nothing, leave the new parent connection
                    //let it go back to the prior loop
                }
                else
                {
                    asnode.setParent(priorParent);
                    //return;
                }
            }
            else
            {
                //do nothing
            }
        }
    }

    public PathNode optimalNode(List<PathNode> list)
    {
        list.Sort();
        foreach (PathNode node in list)
        {
            if (!closedNodes.Contains(node))
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

    public Point loc;
    public PathNode parent;
    public bool diagonalFromParent;

    public PathNode(Point origin, Point destination, PathNode parent)
    {
        loc = origin;
        h = getManhattan(origin, destination);
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

    private int getManhattan(Point start, Point dest)
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
            return 0;
        }
        else
        {
            return 1;
        }
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