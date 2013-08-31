using UnityEngine;
using System.Collections;

public class DungeonMaster : MonoBehaviour {

    public static void PopulateLevel(Level l)
    {
        if (!l.Populated)
        {
            ForcePopulateLevel(l);
        }
    }

    static void ForcePopulateLevel(Level l)
    {
        l.Populated = true;
    }

    static void PickStartLocation(Level l)
    {
    }

    public void SpawnCreature(string npc, int x, int y)
    {
        NPC n = BigBoss.WorldObject.getNPC(npc);
        GameObject gameObject = Instantiate(Resources.Load(n.Prefab), new Vector3(x, -.5f, y), Quaternion.identity) as GameObject;
        NPC newNPC = gameObject.AddComponent<NPC>();
        newNPC.setData(n);
        newNPC.IsActive = true;
        newNPC.init();
    }
}
