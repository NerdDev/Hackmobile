using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelBuilder : MonoBehaviour
{
    const float _chestBuffer = .10F;

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
        _handlers.Add(GridType.Chest, HandleChest);
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
                space.Deploys = new List<GridDeploy>(new[] { new GridDeploy(level.Theme.Get(space.Type).GO) });
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
            space.Deploys.Add(new GridDeploy(level.Theme.Get(GridType.Floor).GO));
            return;
        }
        // Normal
        space.Deploys = new List<GridDeploy>(new[] { new GridDeploy(level.Theme.Get(GridType.Wall).GO) });
    }

    public static void HandleDoor(Level level, GridSpace space)
    {
        space.Deploys = new List<GridDeploy>(2);
        GridDeploy doorDeploy = new GridDeploy(level.Theme.Get(GridType.Door).GO);
        space.Deploys.Add(doorDeploy);
        space.Deploys.Add(new GridDeploy(level.Theme.Get(GridType.Floor).GO));
        // Normal 
        GridDirection walkableDir;
        GridLocation offsetLocation;
        if (level.AlternatesSides(space.X, space.Y, Draw.WalkableSpace(), out walkableDir))
        {
            bool neg = level.Random.NextBool();
            if (walkableDir == GridDirection.HORIZ)
            {
                doorDeploy.Rotation = neg ? -90 : 90;
            }
            else if (neg)
            {
                doorDeploy.Rotation = 180;
            }
        }
        // Diagonal door
        else if (level.AlternatesCorners(space.X, space.Y, Draw.WalkableSpace(), out walkableDir))
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
        // Offset alternates
        else if (level.AlternateSidesOffset(space.X, space.Y, Draw.Not(Draw.WalkableSpace()), out offsetLocation))
        {
            switch (offsetLocation)
            {
                case GridLocation.LEFT:
                    doorDeploy.Rotation = 90;
                    break;
                case GridLocation.RIGHT:
                    doorDeploy.Rotation = -90;
                    break;
                case GridLocation.TOP:
                    doorDeploy.Rotation = 180;
                    break;
            }
        }
    }

    public static void HandleStairs(Level level, GridSpace space)
    {
        space.Deploys = new List<GridDeploy>(1);
        GridDeploy stairDeploy = new GridDeploy(level.Theme.Get(space.Type).GO);
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

    protected static void HandleChest(Level level, GridSpace space)
    {
        space.Deploys = new List<GridDeploy>(2);
        space.Deploys.Add(new GridDeploy(level.Theme.Get(GridType.Floor).GO));
        ThemeElement chestElement = level.Theme.Get(GridType.Chest);
        GridDeploy chestDeploy = new GridDeploy(chestElement.GO);
        space.Deploys.Add(chestDeploy);
        GridLocation wall;
        //if (level.GetRandomLocationAround(space.X, space.Y, false, level.Random, Draw.WallTypeSpace(), out wall))
        //{ // If wall around, make it flush
        //    PlaceFlush(chestDeploy, chestElement, wall, .05F);
        //}
        //else
        //{ // Place randomly in the middle
            PlaceRandomlyInside(level.Random, chestDeploy, chestElement, _chestBuffer);
        //}
    }
    #endregion

    #region Helpers
    protected static void PlaceFlush(GridDeploy deploy, ThemeElement element, GridLocation loc, float buffer = 0F)
    {
        switch (loc)
        {
            case GridLocation.TOP:
                deploy.Rotation = 180;
                deploy.X = -element.Bounds.center.x;
                deploy.Z = GetInside(element, Axis.Z, deploy.Rotation, buffer);
                break;
            case GridLocation.BOTTOM:
                deploy.X = -element.Bounds.center.x;
                deploy.Z = -GetInside(element, Axis.Z, deploy.Rotation, buffer);
                break;
            case GridLocation.LEFT:
                deploy.Rotation = 90;
                deploy.X = -GetInside(element, Axis.X, deploy.Rotation, buffer);
                deploy.Z = -element.Bounds.center.z;
                break;
            case GridLocation.RIGHT:
                deploy.Rotation = -90;
                deploy.X = GetInside(element, Axis.X, deploy.Rotation, buffer);
                deploy.Z = -element.Bounds.center.z;
                break;
        }
    }

    protected static void PlaceRandomlyInside(System.Random random, GridDeploy deploy, ThemeElement element, float buffer = 0F)
    {
        deploy.Rotation = random.NextAngle();
        deploy.X = RandomInside(random, element, Axis.X, deploy.Rotation, buffer, true);
        deploy.Z = RandomInside(random, element, Axis.Z, deploy.Rotation, buffer, true);
    }

    protected static float RandomInside(System.Random random, ThemeElement element, Axis axis, float yRotation, float buffer = 0F, bool rough = true)
    {
        if (rough)
        {
            float range = 1 - buffer * 2;
            return (random.NextFloat() * range) - (range / 2);
        }
        else
        {
            return GetInside(element, axis, yRotation, buffer) * random.NextNegative() * random.NextFloat();
        }
    }

    protected static float GetInside(ThemeElement element, Axis axis, float yRotation, float buffer = 0F)
    {
        yRotation += element.GO.transform.rotation.y;
        float axisValue;
        float centerShift;
        switch (axis)
        {
            case Axis.X:
                centerShift = -element.Bounds.center.x;
                axisValue = (float)(Math.Abs(element.Bounds.size.x * Math.Cos(yRotation)) + Math.Abs(element.Bounds.size.z * Math.Sin(yRotation)));
                break;
            case Axis.Y:
                axisValue = element.Bounds.size.y;
                centerShift = -element.Bounds.center.y;
                break;
            default:
                centerShift = -element.Bounds.center.z;
                axisValue = (float)(Math.Abs(element.Bounds.size.x * Math.Sin(yRotation)) + Math.Abs(element.Bounds.size.z * Math.Cos(yRotation)));
                break;
        }
        float remaining = 1 - axisValue - buffer;
        if (remaining < 0F)
        {
            return centerShift;
        }
        float ret = centerShift + (remaining / 2);
        return ret;
    }
    #endregion

    public void Combine()
    {
        StaticBatchingUtility.Combine(holder);
    }
}
