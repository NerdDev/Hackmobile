using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WorldObjectManager : MonoBehaviour, IManager {

    public WODictionary<NPC> NPCs { get; protected set; }
    public ItemDictionary Items { get; protected set; }

    public void Initialize()
    {
        BigBoss.Debug.w(Logs.Main, "Starting World Object Manager");
        NPCs = new WODictionary<NPC>();
        Items = new ItemDictionary();
        //Initialize Data Manager by asking for it
        DataManager dm = BigBoss.Data;
    }

    public GameObject Instantiate(WorldObject obj, int x, int y)
    {
        return Instantiate(Resources.Load(obj.Prefab), new Vector3(x, -.5f, y), Quaternion.identity) as GameObject;
    }
}
