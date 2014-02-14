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
        if (space == null || space.Deploys == null) return;
        for (int i = 0; i < space.Deploys.Count; i++)
        {
            GridDeploy deploy = space.Deploys[i];
            if (deploy == null) continue;
            Transform t = deploy.GO.transform;
            GameObject obj = Instantiate(
                deploy.GO,
                new Vector3(x + t.position.x + deploy.X, t.position.y, y + t.position.z + deploy.Y)
                , Quaternion.Euler(new Vector3(t.rotation.x, t.rotation.y + deploy.Rotation, t.rotation.z))) as GameObject;
            obj.transform.parent = holder.transform;
            space.Block = obj;
        }
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
            if (space.Type == GridType.NULL) continue;
            Action<Level, GridSpace> action;
            if (_handlers.TryGetValue(space.Type, out action))
            {
                action(level, space);
            }
            else
            {
                space.Deploys = new List<GridDeploy>(new[] { new GridDeploy(level.Theme.Get(space.Type)) });
            }
        }
    }

    #region Handlers
    #region Doors
    public static void HandleDoor(Level level, GridSpace space)
    {
        space.Deploys = new List<GridDeploy>(2);
        GridDeploy doorDeploy = new GridDeploy(level.Theme.Get(GridType.Door));
        space.Deploys.Add(doorDeploy);
        if (level.Array.AlternatesSides(space.X, space.Y, Draw.WalkableSpace()))
        {
            float neg = level.Random.NextBool() ? -1 : 1;
            if (GridTypeEnum.Walkable(level.Array[space.X - 1, space.Y].Type))
            { // Horizontal walk
                doorDeploy.Rotation = 90 * neg;
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
