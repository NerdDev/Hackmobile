using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DungeonMaster : MonoBehaviour, IManager
{

    public void Initialize()
    {
    }

    public void PopulateLevel(Level l)
    {
        if (!l.Populated)
        {
            ForcePopulateLevel(l);
        }
        try
        {
            PickStartLocation(l);
        }
        catch (Exception e)
        {
            Debug.Log(e.StackTrace);
        }
    }

    void ForcePopulateLevel(Level l)
    {
        l.Populated = true;
        foreach (Room room in l.Layout.GetRooms())
        {
            GridMap map = room.GetFloors();
            Value2D<GridType> space = map.RandomValue(Probability.SpawnRand);
            int wer = 23;
            wer++;
            if (LevelManager.Level[space.x, space.y] != null)
            {
                SpawnCreature("skeleton", space.x, space.y);
            }
        }
    }

    void PickStartLocation(Level l)
    {
        //List<Room> rooms = l.Layout.GetRooms();
        //Room room = rooms[Probability.Rand.Next(rooms.Count)];
        //GridMap map = room.GetFloors();
        //Value2D<GridType> space = map.RandomValue(Probability.Rand);
        //Debug.Log("Space chosen: " + space.x + ", " + space.y);

        BigBoss.PlayerInfo.transform.position = new Vector3(14f, -.5f, 14f);
        BigBoss.PlayerInfo.avatarStartLocation = BigBoss.PlayerInfo.transform.position;
        BigBoss.GetCamera.target = BigBoss.PlayerInfo.transform;
    }

    public void SpawnCreature(string npc, int x, int y)
    {
        BigBoss.Debug.w(DebugManager.Logs.Main, "Spawning");
        NPC n = BigBoss.WorldObject.getNPC(npc);
        GameObject gameObject = Instantiate(Resources.Load(n.Prefab), new Vector3(x, -.5f, y), Quaternion.identity) as GameObject;
        NPC newNPC = gameObject.AddComponent<NPC>();
        newNPC.setData(n);
        newNPC.IsActive = true;
        newNPC.init();
    }
}
