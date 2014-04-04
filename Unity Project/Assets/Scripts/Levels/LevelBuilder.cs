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
        space.Blocks = new List<GameObject>(space.Deploys.Count);
        for (int i = 0; i < space.Deploys.Count; i++)
        {
            GridDeploy deploy = space.Deploys[i];
            if (deploy == null) continue;
            Transform t = deploy.GO.transform;
            GameObject obj = Instantiate(
                deploy.GO,
                new Vector3(space.X + t.position.x + deploy.X, t.position.y + deploy.Y, space.Y + t.position.z + deploy.Z)
                , Quaternion.Euler(new Vector3(t.rotation.x + deploy.XRotation, t.rotation.y + deploy.YRotation, t.rotation.z + deploy.ZRotation))) as GameObject;
            if (deploy.Static)
            {
                obj.transform.parent = staticHolder.transform;
            }
            else
            {
                obj.transform.parent = dynamicHolder.transform;
            }
            obj.transform.localScale = new Vector3(
                deploy.XScale * obj.transform.localScale.x,
                deploy.YScale * obj.transform.localScale.y,
                deploy.ZScale * obj.transform.localScale.z);
            space.Blocks.Add(obj);
        }

        //fog of war
        Vector3 pos = new Vector3(space.X, 0f, space.Y);
        int height = 0;
        if (space.Type == GridType.Wall) height = 0;
        BigBoss.Gooey.RecreateFOW(pos, height);

        //combination, GC
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

    public Container2D<GridSpace> GeneratePrototypes(LevelLayout layout)
    {
        MultiMap<GridSpace> ret = new MultiMap<GridSpace>();
        ThemeElementSpec spec = new ThemeElementSpec()
        {
            GenGrid = layout.Grids,
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
                space = new GridSpace(gen.val.Type, gen.x, gen.y)
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
                Deploy(spec);
            }
        }
        return ret;
    }

    protected void Deploy(ThemeElementSpec spec)
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
            GridSpace space;
            if (!spec.Grid.TryGetValue(d, out space))
            {
                space = new GridSpace(spec.Type, spec.DeployX, spec.DeployY)
                {
                    Theme = spec.Theme
                };
                space.Deploys = new List<GridDeploy>();
                spec.Grid[d] = space;
            }
            spec.Space = space;
            spec.DeployX = d.x;
            spec.DeployY = d.y;
            foreach (GenDeploy d2 in d.val)
            {
                spec.GenDeploy = d2;
                Deploy(spec);
            }
        }
    }

    protected bool HandleEmptyDeploy(GenSpace space, System.Random rand)
    {
        space.Deploys = new List<GenDeploy>(1);
        ThemeElement element;
        switch (space.Type)
        {
            case GridType.Wall:
                element = space.Theme.Wall.SmartElement.Get(rand);
                break;
            case GridType.Door:
                element = space.Theme.Door.SmartElement.Get(rand);
                break;
            case GridType.Chest:
                element = space.Theme.Chest.SmartElement.Get(rand);
                break;
            case GridType.NULL:
                return false;
            case GridType.StairPlace:
            case GridType.Floor:
            case GridType.SmallLoot:
            default:
                element = space.Theme.Floor.SmartElement.Get(rand);
                break;
        }
        if (element == null)
        {
            throw new ArgumentException("Theme " + space.Theme.GetType() + " had no elements for type: " + space.Type);
        }
        GenDeploy deploy = new GenDeploy(element);
        space.AddDeploy(deploy, 0, 0);
        return true;
    }

    public void Combine()
    {
        StaticBatchingUtility.Combine(staticHolder);
    }
}
