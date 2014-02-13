using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelBuilder : MonoBehaviour
{
    private static GameObject holder;

    public Theme Theme { get; set; }

    private int combineCounter = 0;
    private int garbageCollectorCounter = 0;
    private static Dictionary<GridType, Action<Level, GridSpace>> _handlers;

    public static void Initialize()
    {
        if (holder == null)
        {
            holder = new GameObject("Level Block Holder");
        }
        _handlers = new Dictionary<GridType, Action<Level, GridSpace>>();
        _handlers.Add(GridType.Door, HandleDoor);
    }

    public void Instantiate(Value2D<GridSpace> val)
    {
        Instantiate(val.val, val.x, val.y);
    }

    public void Instantiate(GridSpace space, int x, int y)
    {
        if (space == null || space.Prototype.GO == null) return;
        GameObject obj = Instantiate(
            space.Prototype.GO, 
            new Vector3(x, space.Prototype.GO.transform.position.y, y)
            , Quaternion.identity) as GameObject;
        obj.transform.parent = holder.transform;
        space.Block = obj;
        combineCounter++;
        garbageCollectorCounter++;
        if (garbageCollectorCounter > 300)
        {
            System.GC.Collect();
            garbageCollectorCounter = 0;
        }
        if (combineCounter > 20)
        {
            combineCounter = 0;
            Combine();
        }
    }

    public void GeneratePrototypes(Level level)
    {
        foreach (GridSpace space in level)
        {
            space.Prototype = new GridPrototype();
            space.Prototype.GO = Theme.Get(space.Type);
            Action<Level, GridSpace> action;
            if (_handlers.TryGetValue(space.Type, out action))
            {
                action(level, space);
            }
        }
    }

    #region Handlers
    public static void HandleDoor(Level level, GridSpace space)
    {
        //if (level.Array.AlternatesSides(space.X, space.Y, GridTypeEnum.Walkable(space.Type)))
        //{

        //}
    }
    #endregion

    public void Combine()
    {
        StaticBatchingUtility.Combine(holder);
    }
}
