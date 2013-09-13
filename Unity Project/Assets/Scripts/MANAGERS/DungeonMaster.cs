using UnityEngine;
using System.Collections;

public class DungeonMaster : MonoBehaviour, IManager {

    public void Initialize()
    {
    }

    public void PopulateLevel(Level l)
    {
        if (!l.Populated)
        {
            ForcePopulateLevel(l);
        }
    }

    void ForcePopulateLevel(Level l)
    {
        l.Populated = true;
        foreach (Room room in l.Layout.GetRooms())
        {
            GridMap map = room.GetFloors();
            Value2D<GridType> space = map.RandomValue(Probability.SpawnRand);
            SpawnCreature("skeleton_knight", space.x, space.y);
        }
    }

    void PickStartLocation(Level l)
    {
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
