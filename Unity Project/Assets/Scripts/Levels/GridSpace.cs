using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSpace
{
    public GridType Type { get; private set; }
    public GameObject Block { get; set; }
    private List<WorldObject> _freeObjects;
    private List<WorldObject> _blockingObjects;

    public GridSpace(GridType type)
    {
        this.Type = type;
    }

    #region Accessors
    public void Put(WorldObject obj)
    {
        // Implement sorting later
        PutFree(obj);
    }

    protected void PutFree(WorldObject obj)
    {
        if (_freeObjects == null)
        {
            _freeObjects = new List<WorldObject>();
        }
        _freeObjects.Add(obj);
    }

    protected void PutBlocking(WorldObject obj)
    {
        if (_blockingObjects == null)
        {
            _blockingObjects = new List<WorldObject>();
        }
        _blockingObjects.Add(obj);
    }

    public void Remove(WorldObject obj)
    {
        RemoveFree(obj);
        RemoveBlocked(obj);
    }

    protected void RemoveFree(WorldObject obj)
    {
        if (_freeObjects != null)
            _freeObjects.Remove(obj);
    }

    protected void RemoveBlocked(WorldObject obj)
    {
        if (_blockingObjects != null)
            _blockingObjects.Remove(obj);
    }
    #endregion

    #region isChecks
    public bool Accept(WorldObject obj)
    {
        return true;
    }

    public bool IsBlocked()
    {
        return _blockingObjects != null && _blockingObjects.Count > 0;
    }

    public bool HasNonBlocking()
    {
        return _freeObjects != null && _freeObjects.Count > 0;
    }

    public bool HasObject()
    {
        return IsBlocked() || HasNonBlocking();
    }

    public bool IsEmpty()
    {
        return !HasObject();
    }
    #endregion

    #region getLists
    public List<WorldObject> GetContained()
    {
        var ret = new List<WorldObject>();
        if (_freeObjects != null)
            ret.AddRange(_freeObjects);
        if (_blockingObjects != null)
            ret.AddRange(_blockingObjects);
        return ret;
    }

    public List<WorldObject> GetFreeObjects()
    {
        var ret = new List<WorldObject>();
        if (_freeObjects != null)
            ret.AddRange(_freeObjects);
        return ret;
    }

    public List<WorldObject> GetBlockingObjects()
    {
        var ret = new List<WorldObject>();
        if (_blockingObjects != null)
            ret.AddRange(_blockingObjects);
        return ret;
    }
    #endregion

    public static GridSpace[,] Convert(GridArray arr)
    {
        GridSpace[,] arrOut = new GridSpace[arr.getHeight(), arr.getWidth()];
        foreach (Value2D<GridType> val in arr)
        {
            arrOut[val.y, val.x] = new GridSpace(val.val);
        }
        return arrOut;
    }
}
