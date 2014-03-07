using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelBuilder : MonoBehaviour
{
    private static GameObject holder;
    private int combineCounter = 0;
    private int garbageCollectorCounter = 0;

    public static void Initialize()
    {
        if (holder == null)
        {
            holder = new GameObject("Level Block Holder");
        }
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

    public Container2D<GridSpace> GeneratePrototypes(LevelLayout layout)
    {
        MultiMap<GridSpace> ret = new MultiMap<GridSpace>();
        ThemeElementSpec spec = new ThemeElementSpec()
        {
            Grid = layout.Grids,
            Random = layout.Random
        };
        foreach (Value2D<GenSpace> gen in layout.Grids)
        {
            if (gen.val.Deploys == null)
            {
                HandleEmptyDeploy(gen.val, spec.Random);
            }
            GridSpace space = new GridSpace(gen.val.Type, gen.x, gen.y)
            {
                Theme = gen.val.Theme
            };
            space.Deploys = new List<GridDeploy>(gen.val.Deploys.Count);
            ret[gen] = space;
            spec.Theme = gen.val.Theme;
            spec.GenSpace = gen.val;
            spec.X = gen.x;
            spec.Y = gen.y;
            spec.Space = space;
            foreach (GenDeploy genDeploy in gen.val.Deploys)
            {
                spec.GenDeploy = genDeploy;
                genDeploy.Element.PreDeployTweaks(spec);
                GridDeploy deploy = new GridDeploy(genDeploy.Element.GO)
                {
                    Rotation = genDeploy.Rotation,
                    X = genDeploy.X,
                    Y = genDeploy.Y,
                    Z = genDeploy.Z
                };
                space.Deploys.Add(deploy);
            }
        }
        return ret;
    }

    protected bool HandleEmptyDeploy(GenSpace space, System.Random rand)
    {
        space.Deploys = new List<GenDeploy>(1);
        ThemeElement element;
        switch (space.Type)
        {
            case GridType.Wall:
                element = space.Theme.Wall.Random(rand);
                break;
            case GridType.StairPlace:
            case GridType.Floor:
                element = space.Theme.Floor.Random(rand);
                break;
            case GridType.Door:
                element = space.Theme.Door.Random(rand);
                break;
            case GridType.StairDown:
                element = space.Theme.StairDown.Random(rand);
                break;
            case GridType.StairUp:
                element = space.Theme.StairUp.Random(rand);
                break;
            case GridType.Chest:
                element = space.Theme.Chest.Random(rand);
                break;
            default:
                return false;
        }
        if (element == null)
        {
            throw new ArgumentException("Theme " + space.Theme.GetType() + " had no elements for type: " + space.Type);
        }
        GenDeploy deploy = new GenDeploy(element);
        space.AddDeploy(deploy);
        return true;
    }

    public void Combine()
    {
        StaticBatchingUtility.Combine(holder);
    }
}
