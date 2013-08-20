using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : IEnumerable<Value2D<GridSpace>>
{
    private LevelLayout Layout { get; set; }
    public bool Populated { get; set; }
    private GridSpace[,] Arr;
    public Surrounding<GridSpace> surr;

    public Level(LevelLayout layout)
    {
        Layout = layout;
        Arr = GridSpace.Convert(layout.GetArray());
        loadSurrounding();
    }

    public GridSpace this[int x, int y]
    {
        get
        {
            if (x < Arr.GetLength(1) && y < Arr.GetLength(0))
            {
                return Arr[y, x];
            }
            return null;
        }
    }

    #region ConvenienceFunctions
    public void Put(int x, int y, WorldObject obj)
    {
        this[x,y].Put(obj);
    }

    public void Remove(int x, int y, WorldObject obj)
    {
        this[x,y].Remove(obj);
    }

    public void Move(WorldObject obj, int xFrom, int yFrom, int xTo, int yTo)
    {
        Remove(xFrom, yFrom, obj);
        Put(xTo, yTo, obj);
    }

    public bool Accept(int x, int y, WorldObject obj)
    {
        return this[x,y].Accept(obj);
    }

    public bool IsBlocked(int x, int y)
    {
        return this[x,y].IsBlocked();
    }

    public bool HasNonBlocking(int x, int y)
    {
        return this[x,y].HasNonBlocking();
    }

    public bool HasObject(int x, int y)
    {
        return this[x,y].HasObject();
    }

    public bool IsEmpty(int x, int y)
    {
        return this[x,y].IsEmpty();
    }

    public List<WorldObject> GetContained(int x, int y)
    {
        return this[x,y].GetContained();
    }

    public List<WorldObject> GetFreeObjects(int x, int y)
    {
        return this[x,y].GetFreeObjects();
    }

    public List<WorldObject> GetBlockingObjects(int x, int y)
    {
        return this[x,y].GetBlockingObjects();
    }
    #endregion

    public void ToLog(DebugManager.Logs log, params string[] customContent)
    {
        if (DebugManager.logging(log))
        {
            GridArray ga = this;
            ga.ToLog(log, customContent);
        }
    }

    public static implicit operator GridArray(Level lev)
    {
        GridType[,] ret = new GridType[lev.Arr.GetLength(0), lev.Arr.GetLength(1)];
        for (int y = 0; y < lev.Arr.GetLength(0); y++)
        {
            for (int x = 0; x < lev.Arr.GetLength(1); x++)
            {
                if (lev.Arr[y, x] != null)
                    ret[y, x] = lev.Arr[y, x];
            }
        }
        return new GridArray(ret);
    }

    public IEnumerator<GridSpace> GetBasicEnumerator()
    {
        for (int y = 0; y < Arr.GetLength(0); y++)
        {
            for (int x = 0; x < Arr.GetLength(1); x++)
            {
                yield return Arr[x, y];
            }
        }
    }

    public IEnumerator<Value2D<GridSpace>> GetEnumerator()
    {
        for (int y = 0; y < Arr.GetLength(0); y++)
        {
            for (int x = 0; x < Arr.GetLength(1); x++)
            {
                yield return new Value2D<GridSpace>(x, y, Arr[y, x]);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    void loadSurrounding()
    {
        surr = new Surrounding<GridSpace>(Arr, true);
    }

    public IEnumerable<Value2D<GridSpace>> getSurroundingSpaces(int x, int y)
    {
        surr.Load(x, y);
        foreach (Value2D<GridSpace> val in surr)
        {
            yield return val;
        }
    }
}
