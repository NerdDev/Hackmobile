using System;
using UnityEngine;

public static class TestManager
{
    public static void Start()
    {
        Debug.Log("Running KC Method");
        KurtisMethod();
    }

    private static void KurtisMethod()
    {
        /*
         * Initializes some test spawns and places the Player object.
         */
        Value2D<GridSpace> loc = BigBoss.DungeonMaster.PickSpawnableLocation(BigBoss.Levels.Level);
        BigBoss.PlayerInfo.transform.position = new Vector3(loc.x, -.5f, loc.y);

        for (int i = 0; i < 4; i++)
        {
            loc = BigBoss.DungeonMaster.PickSpawnableLocation(BigBoss.Levels.Level);
            BigBoss.DungeonMaster.SpawnNPC(new Point(loc.x, loc.y), "skeleton_knight");
        }
        for (int i = 0; i < 4; i++)
        {
            loc = BigBoss.DungeonMaster.PickSpawnableLocation(BigBoss.Levels.Level);
            BigBoss.DungeonMaster.SpawnNPC(new Point(loc.x, loc.y), "giant_spider");
        }

        #region Miscellaneous assignations
        /*
         * Assigns the main camera position.
         *
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        maxCamera mc = camera.AddComponent<maxCamera>();
        mc.maxDistance = 10;
        mc.minDistance = 2;
        mc.yMaxLimit = 75;
        mc.yMinLimit = 25;
        mc.target = BigBoss.PlayerInfo.transform;

        /*
         * Assigns FOW information.
         *
        GameObject FOW = new GameObject();
        FOWSystem fowsys = FOW.AddComponent<FOWSystem>();
        fowsys.blurIterations = 0;
        fowsys.heightRange = new Vector2(-2f, 10f);

        FOWEffect foweff = camera.AddComponent<FOWEffect>();
        foweff.exploredColor = Color.black;
        foweff.unexploredColor = Color.black;
        foweff.shader = Shader.Find("FOW");
        */
        #endregion
    }
}
