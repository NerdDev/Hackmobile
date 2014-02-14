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
        _handlers.Add(GridType.StairUp, HandleStairs);
        _handlers.Add(GridType.StairDown, HandleStairs);
        _handlers.Add(GridType.Wall, HandleWall);
    }

    public void Instantiate(Value2D<GridSpace> val)
    {
        Instantiate(val.val, val.x, val.y);
    }

    public void Instantiate(GridSpace space, int x, int y)
    {
        if (space == null || space.Deploys == null) return;
        space.Blocks = new List<GameObject>(space.Deploys.Count);
        for (int i = 0; i < space.Deploys.Count; i++)
        {
            GridDeploy deploy = space.Deploys[i];
            if (deploy == null) continue;
            Transform t = deploy.GO.transform;
            GameObject obj = Instantiate(
                deploy.GO,
                new Vector3(x + t.position.x + deploy.X, t.position.y + deploy.Y, y + t.position.z + deploy.Z)
                , Quaternion.Euler(new Vector3(t.rotation.x, t.rotation.y + deploy.Rotation, t.rotation.z))) as GameObject;
            obj.transform.parent = holder.transform;
            space.Blocks.Add(obj);
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
        foreach (GridSpace space in level.GetEnumerateValues())
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
    public static void HandleWall(Level level, GridSpace space)
    {
        // Pillar
        if (level.DrawAround(space.X, space.Y, true, Draw.FloorTypeSpace()))
        {
            space.Deploys = new List<GridDeploy>(2);
            GridDeploy pillarDeploy = new GridDeploy(level.Theme.Pillar);
            space.Deploys.Add(pillarDeploy);
            space.Deploys.Add(new GridDeploy(level.Theme.Get(GridType.Floor)));
            return;
        }
        // Normal
        space.Deploys = new List<GridDeploy>(new [] { new GridDeploy(level.Theme.Get(GridType.Wall)) });
    }

    public static void HandleDoor(Level level, GridSpace space)
    {
        space.Deploys = new List<GridDeploy>(2);
        GridDeploy doorDeploy = new GridDeploy(level.Theme.Get(GridType.Door));
        space.Deploys.Add(doorDeploy);
        space.Deploys.Add(new GridDeploy(level.Theme.Get(GridType.Floor)));
        // Normal 
        GridDirection walkableDir;
        if (level.Array.AlternatesSides(space.X, space.Y, Draw.WalkableSpace(), out walkableDir))
        {
            bool neg = level.Random.NextBool();
            if (walkableDir == GridDirection.HORIZ)
            {
                doorDeploy.Rotation = neg ? -90 : 90;
            }
            else
            {
                doorDeploy.Rotation = 180;
            }
        }
        // Diagonal door
        else if (level.Array.AlternatesCorners(space.X, space.Y, Draw.WalkableSpace(), out walkableDir))
        {
            doorDeploy.Rotation = 45;
            doorDeploy.X = -0.25F;
            if (walkableDir == GridDirection.DIAGTLBR)
            {
                doorDeploy.Rotation *= -1;
                doorDeploy.X *= -1;
            }
            doorDeploy.Z = 0.25F;
        }
    }

    public static void HandleStairs(Level level, GridSpace space)
    {
        space.Deploys = new List<GridDeploy>(1);
        GridDeploy stairDeploy = new GridDeploy(level.Theme.Get(space.Type));
        space.Deploys.Add(stairDeploy);
        Value2D<GridSpace> val;
        if (level.Array.GetPointAround(space.X, space.Y, false, Draw.IsType(GridType.StairPlace), out val))
        {
            val.x -= space.X;
            val.y -= space.Y;
            if (val.x == 1)
            {
                stairDeploy.Rotation = 90;
            }
            else if (val.x == -1)
            {
                stairDeploy.Rotation = -90;
            }
            else if (val.y == -1)
            {
                stairDeploy.Rotation = 180;
            }
        }
        if (space.Type == GridType.StairDown)
        {
            stairDeploy.Y = -1;
        }
    }
    #endregion

    public void Combine()
    {
        StaticBatchingUtility.Combine(holder);
    }
}
