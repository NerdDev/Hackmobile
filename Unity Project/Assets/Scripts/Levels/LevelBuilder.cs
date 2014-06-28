using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;

public class LevelBuilder : MonoBehaviour
{
    public static GameObject StaticHolder;
    public static GameObject DynamicHolder;
    public Queue<GridSpace> InstantiationQueue = new Queue<GridSpace>();
    private MultiMap<List<DelayedLevelDeploy>> delayedDeployEvents = new MultiMap<List<DelayedLevelDeploy>>();

    // Area batching
    public const int BatchRectRadius = 4;
    public const int BatchRectDiameter = BatchRectRadius * 2;

    public static void Initialize()
    {
        StaticHolder = new GameObject("Static Block Holder");
        DynamicHolder = new GameObject("Dynamic Block Holder");
        var gameObj = new GameObject("Block Holder");
        StaticHolder.transform.parent = DynamicHolder.transform;
        DynamicHolder.transform.parent = StaticHolder.transform;
    }

    public void Instantiate(GridSpace space)
    {
        if (space == null || space.Deploys == null) return;
        space.Blocks = new List<GameObject>(space.Deploys.Count);
        AreaBatchMapper batch;
        if (!space.Level.BatchMapper.TryGetValue(space, out batch))
        {
            batch = new AreaBatchMapper(space.Level, space);
        }
        List<GridDeploy> delayed = new List<GridDeploy>(space.Deploys.Count);
        for (int i = 0; i < space.Deploys.Count; i++)
        {
            GridDeploy deploy = space.Deploys[i];
            if (deploy == null) continue;
            if (space.InstantiationState == InstantiationState.DelayedInstantiation)
            {
                if (deploy.DelayDeployment)
                {
                    GenerateDeploy(deploy, space, batch);
                }
            }
            else
            {
                if (deploy.DelayDeployment)
                {
                    delayed.Add(deploy);
                }
                else
                {
                    GenerateDeploy(deploy, space, batch);
                }
            }
        }
        if (space.InstantiationState != InstantiationState.DelayedInstantiation)
        {
            FireDelayedDeploy(space);
        }
        if (delayed.Count > 0)
        {
            if (SetUpDeplayedDeployment(space))
            { // Delay deployment
                space.InstantiationState = InstantiationState.DelayedInstantiation;
                return;
            }
            // Didn't need delay, deploy instead
            foreach (GridDeploy deploy in delayed)
            {
                GenerateDeploy(deploy, space, batch);
            }
        }
        batch.Counter--;
        if (batch.Counter == 0)
        {
            batch.Combine(StaticHolder);
        }
        //update fog of war
        Vector3 pos = new Vector3(space.X, 0f, space.Y);
        BigBoss.Gooey.RecreateFOW(pos, 0);
        space.InstantiationState = InstantiationState.Instantiated;
    }

    public void GenerateDeploy(GridDeploy deploy, GridSpace space, AreaBatchMapper batch)
    {
        Transform t = deploy.GO.transform;
        GameObject obj = Instantiate(
            deploy.GO,
            new Vector3(space.X + t.position.x + deploy.X, t.position.y + deploy.Y, space.Y + t.position.z + deploy.Z)
            , Quaternion.Euler(new Vector3(t.eulerAngles.x + deploy.XRotation, t.eulerAngles.y + deploy.YRotation, t.eulerAngles.z + deploy.ZRotation))) as GameObject;
        foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
        {
            Material material = renderer.material;
            if (material == null) continue;
            Material alternateMaterial;
            string materialName = material.name.Substring(0, material.name.Length - 11); // Trim " (instance")
            if (space.Theme.AlternateMaterialsMap.TryGetValue(materialName, out alternateMaterial))
            {
                renderer.sharedMaterial = alternateMaterial;
            }
        }
        if (deploy.Static)
        {
            obj.transform.parent = batch.StaticSpaceHolder.transform;
        }
        else
        {
            obj.transform.parent = batch.DynamicSpaceHolder.transform;
        }
        obj.transform.localScale = new Vector3(
            deploy.XScale * obj.transform.localScale.x,
            deploy.YScale * obj.transform.localScale.y,
            deploy.ZScale * obj.transform.localScale.z);
        if (deploy.ColliderPlacementQueue != null
            && deploy.ColliderPlacementQueue.Length > 0)
        {
            Renderer render = obj.GetComponentInChildren<Renderer>();
            Bounds bounds = render.bounds;
            Vector3 extents = bounds.extents;
            for (int i = 0; i < deploy.ColliderPlacementQueue.Length; i++)
            {
                AxisDirection dir = deploy.ColliderPlacementQueue[i];
                ShiftIntoPlace(obj, deploy, dir, render, extents);
            }
        }
        space.Blocks.Add(obj);
        batch.Absorb(obj);
    }

    public MultiMap<GridSpace> GeneratePrototypes(Level level, LevelLayout layout)
    {
        MultiMap<GridSpace> ret = new MultiMap<GridSpace>();
        ThemeElementSpec spec = new ThemeElementSpec()
        {
            GenGrid = layout,
            Random = layout.Random,
            Grid = ret
        };
        foreach (Value2D<GenSpace> gen in spec.GenGrid)
        {
            HandleStandardDeploy(gen.val, spec.Random);
            GridSpace space;
            if (!ret.TryGetValue(gen, out space))
            {
                space = new GridSpace(level, gen.val.Type, gen.x, gen.y)
                {
                    Theme = gen.val.Theme
                };
                space.Deploys = new List<GridDeploy>(gen.val.Deploys.Count);
                ret[gen] = space;
            }
            spec.GenSpace = gen.val;
            spec.Space = space;
            spec.Theme = gen.val.Theme;
            spec.Type = gen.val.Type;
            spec.DeployX = gen.x;
            spec.DeployY = gen.y;
            List<GenDeploy> tmp = new List<GenDeploy>(gen.val.MainDeploys);
            foreach (GenDeploy genDeploy in tmp)
            {
                spec.GenDeploy = genDeploy;
                Deploy(level, spec);
            }
            space.Elements = new List<ThemeElement>(gen.val.Deploys.Count);
            foreach (GenDeploy genDeploy in gen.val.Deploys)
            {
                space.Elements.Add(genDeploy.Element);
            }

        }
        return ret;
    }

    protected bool SetUpDeplayedDeployment(GridSpace space)
    {
        DelayedLevelDeploy delayedDeploy = null;
        space.Level.DrawAround(space.X, space.Y, false, (arr, x, y) =>
        {
            GridSpace around;
            if (arr.TryGetValue(x, y, out around))
            {
                if (around.InstantiationState < InstantiationState.DelayedInstantiation)
                {
                    if (delayedDeploy == null)
                    {
                        delayedDeploy = new DelayedLevelDeploy(space);
                    }
                    delayedDeploy.Counter++;
                    List<DelayedLevelDeploy> deployList;
                    if (!delayedDeployEvents.TryGetValue(x, y, out deployList))
                    {
                        deployList = new List<DelayedLevelDeploy>();
                        delayedDeployEvents[x, y] = deployList;
                    }
                    deployList.Add(delayedDeploy);
                }
            }
            return true;
        });
        return delayedDeploy != null;
    }

    protected void FireDelayedDeploy(GridSpace space)
    {
        List<DelayedLevelDeploy> list;
        if (delayedDeployEvents.TryGetValue(space, out list))
        {
            List<DelayedLevelDeploy> deployed = new List<DelayedLevelDeploy>(list.Count);
            foreach (var delayedDeploy in list)
            {
                delayedDeploy.Counter--;
                if (delayedDeploy.Counter == 0)
                {
                    InstantiationQueue.Enqueue(delayedDeploy.space);
                    deployed.Add(delayedDeploy);
                }
            }
            foreach (var delayedDeploy in deployed)
            {
                list.Remove(delayedDeploy);
            }
            if (list.Count == 0)
            {
                delayedDeployEvents.Remove(space);
            }
        }
    }

    protected void Deploy(Level level, ThemeElementSpec spec)
    {
        if (spec.GenDeploy.Deployed) return;
        spec.GenDeploy.Deployed = true;
        spec.Reset();
        spec.GenDeploy.Element.PreDeployTweaks(spec);
        GridDeploy deploy = new GridDeploy(spec.GenDeploy);

        spec.Space.Deploys.Add(deploy);
        if (spec.Additional.Count == 0) return;
        foreach (var d in spec.Additional.ToList())
        {
            spec.DeployX = d.x;
            spec.DeployY = d.y;
            GridSpace space;
            if (!spec.Grid.TryGetValue(d, out space))
            {
                space = new GridSpace(level, spec.Type, spec.DeployX, spec.DeployY)
                {
                    Theme = spec.Theme
                };
                space.Deploys = new List<GridDeploy>();
                spec.Grid[d] = space;
            }
            spec.Space = space;
            foreach (GenDeploy d2 in d.val)
            {
                spec.GenDeploy = d2;
                Deploy(level, spec);
            }
        }
    }

    protected bool HandleStandardDeploy(GenSpace space, System.Random rand)
    {
        if (space.Deploys == null)
        {
            space.Deploys = new List<GenDeploy>(1);
        }
        SmartThemeElement element;
        switch (space.Type)
        {
            case GridType.Wall:
                space.Theme.Wall.Select(rand, 1, 1, out element, false);
                break;
            case GridType.Chest:
                space.Theme.Chest.Select(rand, 1, 1, out element, false);
                break;
            case GridType.NULL:
                return false;
            case GridType.Door:
            case GridType.StairPlace:
            case GridType.Floor:
            case GridType.SmallLoot:
            default:
                space.Theme.Floor.Select(rand, 1, 1, out element, false);
                break;
        }
        if (element == null)
        {
            throw new ArgumentException("Theme " + space.Theme.GetType() + " had no elements for type: " + space.Type);
        }
        GenDeploy deploy = new GenDeploy(element.Get(rand));
        space.AddDeploy(deploy, 0, 0);
        return true;
    }

    public void InstantiateThings()
    {
        while (InstantiationQueue.Count != 0)
        {
            GridSpace space = InstantiationQueue.Dequeue();
            switch (space.InstantiationState)
            {
                case InstantiationState.WantsDestruction:
                    foreach (GameObject block in space.Blocks)
                    {
                        GameObject.Destroy(block);
                    }
                    AreaBatchMapper batch;
                    if (space.Level.BatchMapper.TryGetValue(space, out batch))
                    {
                        batch.Destroy();
                    }
                    space.InstantiationState = InstantiationState.NotInstantiated;
                    break;
                case InstantiationState.WantsInstantiation:
                case InstantiationState.DelayedInstantiation:
                    Instantiate(space);
                    break;
                case InstantiationState.NotInstantiated:
                case InstantiationState.Instantiated:
                default:
                    continue;
            }
        }
    }

    protected bool ShiftIntoPlace(GameObject obj, GridDeploy deploy, AxisDirection dir, Renderer renderer, Vector3 extents)
    {
        RaycastHit hit;
        if (Physics.Raycast(obj.transform.position, dir.GetVector3(), out hit, 1F))
        {
            obj.transform.position = hit.point;
            return true;
        }
        return false;
    }
}

public class AreaBatchMapper
{
    private List<GameObject> objects = new List<GameObject>();
    public Counter Counter;
    int originX;
    int originY;
    Level level;
    public GameObject StaticSpaceHolder;
    public GameObject DynamicSpaceHolder;
    public AreaBatchMapper(Level level, GridSpace space)
    {
        originX = GetOrigin(space.X);
        originY = GetOrigin(space.Y);
        StaticSpaceHolder = new GameObject(originX + "," + originY);
        StaticSpaceHolder.transform.parent = LevelBuilder.StaticHolder.transform;
        DynamicSpaceHolder = new GameObject(originX + "," + originY);
        DynamicSpaceHolder.transform.parent = LevelBuilder.DynamicHolder.transform;
        this.level = level;
        level.DrawRect(
            originX,
            originX + LevelBuilder.BatchRectDiameter - 1,
            originY,
            originY + LevelBuilder.BatchRectDiameter - 1,
            Draw.PointContainedIn<GridSpace>().IfThen(Draw.Count<GridSpace>(out Counter).And((arr, x2, y2) =>
            {
                level.BatchMapper[x2, y2] = this;
                return true;
            })));
    }

    public int GetOrigin(int i)
    {
        if (i >= 0)
        {
            i /= LevelBuilder.BatchRectDiameter;
            i *= LevelBuilder.BatchRectDiameter;
        }
        else
        {
            i++;
            i /= LevelBuilder.BatchRectDiameter;
            i *= LevelBuilder.BatchRectDiameter;
            i -= LevelBuilder.BatchRectDiameter;
        }
        return i;
    }

    public void Absorb(GameObject obj)
    {
        objects.Add(obj);
    }

    public void Combine(GameObject holder)
    {
        StaticBatchingUtility.Combine(objects.ToArray(), holder);
        RemoveSelf();
    }

    public void RemoveSelf()
    {
        level.BatchMapper.DrawRect(
            originX,
            originX + LevelBuilder.BatchRectDiameter - 1,
            originY,
            originY + LevelBuilder.BatchRectRadius - 1,
            Draw.Remove<AreaBatchMapper>());
    }

    public void Destroy()
    {
        RemoveSelf();
        level.DrawRect(
            originX,
            originX + LevelBuilder.BatchRectDiameter - 1,
            originY,
            originY + LevelBuilder.BatchRectRadius - 1,
            new DrawAction<GridSpace>((arr, x, y) =>
            {
                GridSpace space;
                if (arr.TryGetValue(x, y, out space))
                {
                    space.DestroyGridSpace();
                }
                return true;
            }));
    }
}