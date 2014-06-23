using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class LevelBuilder : MonoBehaviour
{
    public static GameObject StaticHolder;
    public static GameObject DynamicHolder;

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
        if (space.BlocksCreated) //if instantiate is called and the blocks are created, just enable them
        {
            space.SetActive(true);
            return;
        }
        else //else create the blocks
        {
            space.Blocks = new List<GameObject>(space.Deploys.Count);
            AreaBatchMapper batch;
            if (!space.Level.BatchMapper.TryGetValue(space, out batch))
            {
                batch = new AreaBatchMapper(space.Level, space);
            }
            batch.Counter--;
            for (int i = 0; i < space.Deploys.Count; i++)
            {
                GridDeploy deploy = space.Deploys[i];
                if (deploy == null) continue;
                Transform t = deploy.GO.transform;
                GameObject obj = Instantiate(
                    deploy.GO,
                    new Vector3(space.X + t.position.x + deploy.X, t.position.y + deploy.Y, space.Y + t.position.z + deploy.Z)
                    , Quaternion.Euler(new Vector3(t.eulerAngles.x + deploy.XRotation, t.eulerAngles.y + deploy.YRotation, t.eulerAngles.z + deploy.ZRotation))) as GameObject;
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
                space.Blocks.Add(obj);
                batch.Absorb(obj);
            }
            if (batch.Counter == 0)
            {
                batch.Combine(StaticHolder);
            }
            space.BlocksCreated = true; //space has created blocks
        }
        //update fog of war
        Vector3 pos = new Vector3(space.X, 0f, space.Y);
        BigBoss.Gooey.RecreateFOW(pos, 0);

        //GarbageCollect(); //not currently used now
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
            if (gen.val.Deploys == null)
            {
                HandleEmptyDeploy(gen.val, spec.Random);
            }
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
        }
        return ret;
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

    protected bool HandleEmptyDeploy(GenSpace space, System.Random rand)
    {
        space.Deploys = new List<GenDeploy>(1);
        SmartThemeElement element;
        switch (space.Type)
        {
            case GridType.Wall:
                space.Theme.Wall.Select(rand, 1, 1, out element, false);
                break;
            case GridType.Door:
                space.Theme.Door.Select(rand, 1, 1, out element, false);
                break;
            case GridType.Chest:
                space.Theme.Chest.Select(rand, 1, 1, out element, false);
                break;
            case GridType.NULL:
                return false;
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
    public AreaBatchMapper (Level level, GridSpace space)
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
        //#region DEBUG
        //if (BigBoss.Debug.logging())
        //{
        //    level.BatchMapper.ToLog(BigBoss.Debug.Get(Logs.Main), "Batch map for " + space.X + ", " + space.Y + " on origin " + originX + ", " + originY);
        //}
        //#endregion
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
        level.BatchMapper.DrawRect(
            originX,
            originX + LevelBuilder.BatchRectDiameter - 1,
            originY,
            originY + LevelBuilder.BatchRectRadius - 1,
            Draw.Remove<AreaBatchMapper>());
        //#region DEBUG
        //if (BigBoss.Debug.logging())
        //{
        //    level.BatchMapper.ToLog(BigBoss.Debug.Get(Logs.Main), "Removing origin " + originX + ", " + originY);
        //}
        //#endregion
    }
}