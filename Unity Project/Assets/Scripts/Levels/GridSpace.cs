using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSpace
{
    public GridType Type { get; private set; }
    public GameObject Block { get; set; }
    public int X { get { return (int)Block.transform.position.x; } }
    public int Y { get { return (int)Block.transform.position.z; } }
    public WorldObject RandomContainedObj { 
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
    private ItemChest _chest;
    public bool Spawnable { get { return GetBlockingObjects().Count == 0 && Type == GridType.Floor; } }

    public GridSpace(GridType type)
    {
        this.Type = type;
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
            if (obj.IsNotAFreaking<Player>())
            {
                ColliderTrigger(false);
            }
        }
        else if (obj is Item)
        {
            PutInChest(obj as Item);
        }
        else
        {
            PutFree(obj);
        }
    }

    public void PutInChest(Item i)
    {
        if (_chest == null)
        {
            _chest = (GameObject.Instantiate(BigBoss.Gooey.ChestPrefab) as GameObject).GetComponent<ItemChest>();
            _chest.Location = this;
            _chest.init();
        }
        _chest.items.Add(i);
    }

    public bool RemoveFromChest(Item i)
    {
        if (_chest != null)
        {
            _chest.items.Remove(i);
            if (_chest.items.Count == 0)
            {
                GameObject.Destroy(_chest.gameObject);
                _chest = null;
                return true;
            }
        }
        return false;
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
        if (obj is Item) { RemoveFromChest(obj as Item); }
        if (_blockingObjects.Count == 0)
        {
            ColliderTrigger(true);
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

    public static GridSpace[,] Convert(GridArray arr)
    {
        GridSpace[,] arrOut = new GridSpace[arr.getHeight(), arr.getWidth()];
        foreach (Value2D<GridType> val in arr)
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
            arrOut[val.y, val.x] = new GridSpace(val.val);
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
}
