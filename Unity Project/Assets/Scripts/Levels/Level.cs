using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level
{
    protected LevelLayout Layout;
    public GridArray Arr { get; private set; }
    protected List<WorldObject>[,] ContainedObjs;

    public Level(LevelLayout layout)
    {
        Layout = layout;
        Arr = layout.GetArray();
        ContainedObjs = new List<WorldObject>[Arr.getHeight(), Arr.getWidth()];
    }

    public void Move(WorldObject obj, int fromX, int fromY, int toX, int toY)
    {
        Remove(obj, fromX, fromY);
        Put(obj, toX, toY);
    }

    public void Put(WorldObject obj, int x, int y)
    {
        List<WorldObject> space = Get(x, y);
        space.Add(obj);
    }

    public List<WorldObject> Get(int x, int y)
    {
        List<WorldObject> space = ContainedObjs[y, x];
        if (space == null)
        {
            ContainedObjs[y, x] = new List<WorldObject>();
        }
        return space;
    }

    public bool Remove(WorldObject obj, int x, int y)
    {
        List<WorldObject> space = Get(x, y);
        return space.Remove(obj);
    }
}
