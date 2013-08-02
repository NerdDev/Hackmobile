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
}
