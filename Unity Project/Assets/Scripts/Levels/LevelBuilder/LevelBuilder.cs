using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelBuilder : MonoBehaviour
{
    private static GameObject holder;
    public Theme Theme;
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
        if (space == null || space.Deploy.GO == null) return;
        GridDeploy deploy = space.Deploy;
        Transform t = space.Deploy.GO.transform;
        if (deploy.Rotation > 0)
        {
            int wer = 23;
            wer++;
        }
        GameObject obj = Instantiate(
            deploy.GO,
            new Vector3(x + t.position.x + deploy.X, t.position.y, y + t.position.z + deploy.Y)
            , Quaternion.Euler(new Vector3(t.rotation.x, t.rotation.y + deploy.Rotation, t.rotation.z))) as GameObject;
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
            space.Deploy = new GridDeploy();
            space.Deploy.GO = Theme.Get(space.Type);
            Action<Level, GridSpace> action;
            if (_handlers.TryGetValue(space.Type, out action))
            {
                action(level, space);
            }
        }
    }

    #region Handlers
    #region Doors
    public static void HandleDoor(Level level, GridSpace space)
    {
        space.Deploy.GO = level.Theme.Get(GridType.Door);
        if (level.Array.AlternatesSides(space.X, space.Y, Draw.WalkableSpace()))
        {
            float neg = level.Random.NextBool() ? -1 : 1;
            if (GridTypeEnum.Walkable(level.Array[space.X - 1, space.Y].Type))
            { // Horizontal walk
                space.Deploy.Rotation = 90 * neg;
            }
        }
    }
    #endregion
    #endregion

    public void Combine()
    {
        StaticBatchingUtility.Combine(holder);
    }
}
