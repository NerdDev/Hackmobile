using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSpace
{
    public GridType Type { get; private set; }
    public GameObject Block;
    public int X { get; protected set; }
    public int Y { get; protected set; }
    public WorldObject RandomContainedObj
    {
        get
        {
            List<WorldObject> tmp = new List<WorldObject>();
            tmp.AddRange(_freeObjects);
            tmp.AddRange(_blockingObjects);
            return tmp.Random(Probability.Rand);
        }
    }
    private List<WorldObject> _freeObjects;
    private List<WorldObject> _blockingObjects;
    internal Inventory inventory = new Inventory();
    private ItemChest _chest;
    public bool Spawnable { get { return GetBlockingObjects().Count == 0 && Type == GridType.Floor; } }

    public GridSpace(GridType type, int x, int y)
    {
        this.Type = type;
        X = x;
        Y = y;
    }

    public void ColliderTrigger(bool on)
    {
        if (Block != null && Block.collider != null)
        {
            Block.collider.isTrigger = on;
        }
    }

    #region Accessors
    public void Put(WorldObject obj)
    {
        // Implement sorting later
        if (obj is NPC)
        {
            PutBlocking(obj);
        }
        else if (obj is Item)
        {
            PutInventory(obj as Item);
        }
        else
        {
            PutFree(obj);
        }
    }

    public void Put(WorldObject obj, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Put(obj);
        }
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

    internal void PutInventory(Item i)
    {
        if (_chest == null)
        {
            _chest = (GameObject.Instantiate(BigBoss.Gooey.ChestPrefab) as GameObject).GetComponent<ItemChest>();
            _chest.Location = this;
            _chest.init();
        }
        inventory.Add(i);
    }

    public void Remove(WorldObject obj)
    {
        RemoveFree(obj);
        RemoveBlocked(obj);
        if (obj is Item) { RemoveInventory(obj as Item); }
    }

    public void Remove(WorldObject obj, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Remove(obj);
        }
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

    internal bool RemoveInventory(Item i)
    {
        if (_chest != null)
        {
            inventory.Remove(i);
            if (inventory.IsEmpty())
            {
                GameObject.Destroy(_chest.gameObject);
                _chest = null;
                return true;
            }
        }
        return false;
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

    public void SetActive(bool on)
    {
        if (Block != null)
            Block.SetActive(on);
    }

    public void WrapObjects(bool wrap)
    {
        foreach (WorldObject wo in GetBlockingObjects())
        {
            if (wo.IsNotAFreaking<Player>())
            {
                wo.IsActive = wrap;
                wo.Wrap(wrap);
            }
        }
        foreach (WorldObject wo in GetFreeObjects())
        {
            wo.IsActive = wrap;
            wo.Wrap(wrap);
        }
    }

    public static Array2D<GridSpace> Convert(Container2D<GridType> container)
    {
        Array2D<GridSpace> arrOut = new Array2D<GridSpace>(container.Bounding);
        foreach (Value2D<GridType> val in container)
        {
            if (val == null) continue;
            switch (val.val)
            {
                case GridType.Path_Horiz:
                case GridType.Path_Vert:
                case GridType.Path_RT:
                case GridType.Path_LT:
                case GridType.Path_LB:
                case GridType.Path_RB:
                    val.val = GridType.Floor;
                    break;
            }
            arrOut[val.x, val.y] = new GridSpace(val.val, val.x, val.y);
        }
        return arrOut;
    }

    public static implicit operator GridType(GridSpace space)
    {
        return space.Type;
    }

    public static implicit operator Point(GridSpace space)
    {
        return new Point(space.X, space.Y);
    }

    public override bool Equals(object obj)
    {
        GridSpace rhs = obj as GridSpace;
        if (rhs == null) return false;
        return (X == rhs.X) && (Y == rhs.Y);
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() ^ Y.GetHashCode();
    }
}
