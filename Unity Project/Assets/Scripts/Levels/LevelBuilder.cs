using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class LevelBuilder : MonoBehaviour
{
    private static GameObject staticHolder;
    private static GameObject dynamicHolder;
    private int combineCounter = 0;
    private int garbageCollectorCounter = 0;
    private HashSet<GameObject> blocks = new HashSet<GameObject>();

    public static void Initialize()
    {
        if (staticHolder == null)
        {
            staticHolder = new GameObject("Static Block Holder");
            dynamicHolder = new GameObject("Dynamic Block Holder");
            var gameObj = new GameObject("Block Holder");
            staticHolder.transform.parent = dynamicHolder.transform;
            dynamicHolder.transform.parent = staticHolder.transform;
        }
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
            GameObject staticSpaceHolder = null;
            GameObject dynamicSpaceHolder = null;
            space.Blocks = new List<GameObject>(space.Deploys.Count);
            List<GridDeploy> delayed = new List<GridDeploy>(space.Deploys.Count);
            for (int i = 0; i < space.Deploys.Count; i++)
            {
                GridDeploy deploy = space.Deploys[i];
                if (deploy == null) continue;
                if (deploy.DelayDeployment)
                {
                    delayed.Add(deploy);
                    continue;
                }
                GenerateDeploy(space, deploy, staticSpaceHolder, dynamicSpaceHolder);
            }
            // Generate delayed
            for (int i = 0; i < delayed.Count; i++)
            {
                GridDeploy deploy = space.Deploys[i];
                GenerateDeploy(space, deploy, staticSpaceHolder, dynamicSpaceHolder);
            }
            space.Deploys = null; // Clear deployed deploys
            space.BlocksCreated = true; //space has created blocks
        }
        //update fog of war
        Vector3 pos = new Vector3(space.X, 0f, space.Y);
        BigBoss.Gooey.RecreateFOW(pos, 0);

    }

    protected void GenerateDeploy(GridSpace space, GridDeploy deploy, GameObject staticSpaceHolder, GameObject dynamicSpaceHolder)
    {
        Transform t = deploy.GO.transform;
        GameObject obj = Instantiate(
            deploy.GO,
            new Vector3(space.X + t.position.x + deploy.X, t.position.y + deploy.Y, space.Y + t.position.z + deploy.Z)
            , Quaternion.Euler(new Vector3(t.eulerAngles.x + deploy.XRotation, t.eulerAngles.y + deploy.YRotation, t.eulerAngles.z + deploy.ZRotation))) as GameObject;
        if (deploy.Static)
        {
            if (staticSpaceHolder == null)
            {
                staticSpaceHolder = new GameObject(space.X + "," + space.Y);
                staticSpaceHolder.transform.parent = staticHolder.transform;
            }
            obj.transform.parent = staticSpaceHolder.transform;
        }
        else
        {
            if (dynamicSpaceHolder == null)
            {
                dynamicSpaceHolder = new GameObject(space.X + "," + space.Y);
                dynamicSpaceHolder.transform.parent = dynamicHolder.transform;
            }
            obj.transform.parent = dynamicSpaceHolder.transform;
        }
        obj.transform.localScale = new Vector3(
            deploy.XScale * obj.transform.localScale.x,
            deploy.YScale * obj.transform.localScale.y,
            deploy.ZScale * obj.transform.localScale.z);
        if (deploy.ColliderPlacementQueue != null
            && deploy.ColliderPlacementQueue.Length > 0)
        {
            ColliderEventScript script = obj.AddComponent<ColliderEventScript>();
            for (int i = 0; i < deploy.ColliderPlacementQueue.Length; i++)
            {
                Axis dir = deploy.ColliderPlacementQueue[i];
                ShiftIntoPlace(obj, dir, script);
            }
            Destroy(script);
        }
        space.Blocks.Add(obj);
        CombineBlock(obj);
    }

    protected void ShiftIntoPlace(GameObject obj, Axis dir, ColliderEventScript script)
    {
    }

    private void GarbageCollect()
    {
        garbageCollectorCounter++;
        if (garbageCollectorCounter > 75)
        {
            System.GC.Collect(0);
            garbageCollectorCounter = 0;
        }
    }

    private void CombineBlock(GameObject obj)
    {
        blocks.Add(obj);
        combineCounter++;

        if (combineCounter > 40)
        {
            Combine();
            blocks.Clear();
            combineCounter = 0;
        }
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

    public void Remove(GameObject obj)
    {
        blocks.Remove(obj);
    }

    public void Combine()
    {
        StaticBatchingUtility.Combine(blocks.ToArray(), staticHolder);
    }
}
