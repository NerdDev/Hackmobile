using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level
{
    protected LevelLayout Layout;
    public GridSpace[,] Arr { get; private set; }

    public GridSpace Get(int x, int y)
    {
        if (x < Arr.GetLength(1) && y < Arr.GetLength(0))
        {
            return Arr[y, x];
        }
        return null;
    }

    #region ConvenienceFunctions
    public void Put(int x, int y, WorldObject obj)
    {
        Get(x,y).Put(obj);
    }

    public void Remove(int x, int y, WorldObject obj)
    {
        Get(x, y).Remove(obj);
    }

    public bool Accept(int x, int y, WorldObject obj)
    {
        return Get(x, y).Accept(obj);
    }

    public bool IsBlocked(int x, int y)
    {
        return Get(x, y).IsBlocked();
    }

    public bool HasNonBlocking(int x, int y)
    {
        return Get(x, y).HasNonBlocking();
    }

    public bool HasObject(int x, int y)
    {
        return Get(x, y).HasObject();
    }

    public bool IsEmpty(int x, int y)
    {
        return Get(x, y).IsEmpty();
    }

    public List<WorldObject> GetContained(int x, int y)
    {
        return Get(x, y).GetContained();
    }

    public List<WorldObject> GetFreeObjects(int x, int y)
    {
        return Get(x, y).GetFreeObjects();
    }

    public List<WorldObject> GetBlockingObjects(int x, int y)
    {
        return Get(x, y).GetBlockingObjects();
    }
    #endregion
}
