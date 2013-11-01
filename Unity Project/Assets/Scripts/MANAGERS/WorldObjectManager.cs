using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WorldObjectManager : MonoBehaviour, IManager {

    public WODictionary<NPC> NPCs { get; protected set; }
    public WODictionary<Item> Items { get; protected set; }
    Dictionary<string, List<Item>> itemCategories = new Dictionary<string, List<Item>>();
    Dictionary<string, MaterialType> materials = new Dictionary<string, MaterialType>();

    public void Initialize()
    {
        BigBoss.Debug.w(Logs.Main, "Starting World Object Manager");
        //Initialize Data Manager by asking for it
        DataManager dm = BigBoss.Data;
    }

    void Start()
    {
    }

    public GameObject Instantiate(WorldObject obj, int x, int y)
    {
        return Instantiate(Resources.Load(obj.Prefab), new Vector3(g.X, -.5f, g.Y), Quaternion.identity) as GameObject;
    }
}
