using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSpace : IGridSpace
{
    public GridType Type { get; set; }
    public Theme Theme { get; set; }
    public List<GridDeploy> Deploys;
    public int ThemeElementCount
    {
        get
        {
            if (Deploys == null) return 0;
            return Deploys.Count;
        }
    }
    public List<GameObject> Blocks;
    public Level Level { get; protected set; }
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
    internal ItemChest _chest;
    public bool Spawnable { get { return GetBlockingObjects().Count == 0 && Type == GridType.Floor; } }
    public InstantiationState InstantiationState;

    public GridSpace(Level level, GridType type, int x, int y)
    {
        this.Level = level;
        this.Type = type;
        X = x;
        Y = y;
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
            _chest = (GameObject.Instantiate(BigBoss.Gooey.ChestPrefab,
                (Blocks[0].transform.position + BigBoss.Gooey.ChestPrefab.transform.position),
                BigBoss.Gooey.ChestPrefab.transform.localRotation) as GameObject).GetComponent<ItemChest>();
            //_chest.Location = this;
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
        if (Blocks != null)
        {
            foreach (GameObject block in Blocks)
            {
                block.SetActive(on);
                if (on)
                {
                    FOWRenderers renderer = block.GetComponent<FOWRenderers>();
                    if (renderer != null)
                    {
                        renderer.OnEnable();
                    }
                }
            }
        }
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

    public void Instantiate()
    {
        if (InstantiationState < InstantiationState.WantsInstantiation && FOWSystem.instance.IsVis(X, Y))
        {
            InstantiationState = InstantiationState.WantsInstantiation;
            BigBoss.Levels.Builder.InstantiationQueue.Enqueue(this);
        }
    }

    public void DestroyGridSpace()
    {
        if (InstantiationState > InstantiationState.WantsDestruction)
        {
            InstantiationState = global::InstantiationState.WantsDestruction;
            BigBoss.Levels.Builder.InstantiationQueue.Enqueue(this);
        }
    }

    public double Distance(GridSpace rhs)
    {
        return Distance(rhs.X, rhs.Y);
    }

    public double Distance(int x, int y)
    {
        return Math.Sqrt(Math.Pow(this.X - x, 2) + Math.Pow(this.Y - y, 2));
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

    public IEnumerable<ThemeElement> GetThemeElements()
    {
        if (Deploys != null)
        {
            foreach (var deploy in Deploys)
            {
                yield return deploy.Element;
            }
        }
    }

    public override string ToString()
    {
        return "GridSpace (" + X + "," + Y + ")";
    }
}

public enum InstantiationState
{
    NotInstantiated,
    WantsDestruction,
    WantsInstantiation,
    Instantiated
}