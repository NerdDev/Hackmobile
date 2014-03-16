
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

        //fog of war
        Vector3 pos = new Vector3(x, 0f, y);
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
            spec.Space = space;
            spec.Theme = gen.val.Theme;
            spec.Type = gen.val.Type;
            spec.DeployX = gen.x;
            spec.DeployY = gen.y;
            List<GenDeploy> tmp = new List<GenDeploy>(gen.val.Deploys);
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
        spec.Additional = new MultiMap<List<GenDeploy>>();
        spec.GenDeploy.Deployed = true;
        spec.GenDeploy.Element.PreDeployTweaks(spec);
        GridDeploy deploy = new GridDeploy(spec.GenDeploy.Element.GO)
        {
            Rotation = spec.GenDeploy.Rotation,
            X = spec.GenDeploy.X,
            Y = spec.GenDeploy.Y,
            Z = spec.GenDeploy.Z
        };
        spec.Space.Deploys.Add(deploy);
        if (spec.Additional.Count == 0) return;
        MultiMap<List<GenDeploy>> additional = spec.Additional;
        foreach (var d in additional)
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
                element = space.Theme.Core.Wall.Random(rand);
                break;
            case GridType.Door:
                element = space.Theme.Core.Door.Random(rand);
                break;
            case GridType.StairDown:
                element = space.Theme.Core.StairDown.Random(rand);
                break;
            case GridType.StairUp:
                element = space.Theme.Core.StairUp.Random(rand);
                break;
            case GridType.Chest:
                element = space.Theme.Core.Chest.Random(rand);
                break;
            case GridType.NULL:
                return false;
            case GridType.StairPlace:
            case GridType.Floor:
            case GridType.SmallLoot:
            default:
                element = space.Theme.Core.Floor.Random(rand);
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
        StaticBatchingUtility.Combine(holder);
    }
}
