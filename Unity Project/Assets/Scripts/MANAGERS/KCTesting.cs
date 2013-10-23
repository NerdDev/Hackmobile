using System;
using System.Collections.Generic;
using UnityEngine;

public class KCTesting : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Running KC Method");
        KurtisMethod();
    }

    private void KurtisMethod()
    {
        /*
         * Initializes some test spawns and places the Player object.
         */
        BigBoss.Player.transform.position = new Vector3(83f, -.5f, 58f);
        Item ii = BigBoss.WorldObject.CreateItem("sword1");
        BigBoss.Player.addToInventory(ii);
        BigBoss.Player.equipItem(ii);

        Item food = BigBoss.WorldObject.CreateItem("spoiled bread");
        BigBoss.Player.addToInventory(food, 5);

        Item potion = BigBoss.WorldObject.CreateItem("health potion");
        BigBoss.Player.addToInventory(potion, 3);

        /*
        Value2D<GridSpace> loc;
        for (int i = 0; i < 4; i++)
        {
            loc = BigBoss.DungeonMaster.PickStartLocation(LevelManager.Level);
            BigBoss.DungeonMaster.SpawnCreature(new Point(loc.x, loc.y), "skeleton_knight");
        }
        for (int i = 0; i < 4; i++)
        {
            loc = BigBoss.DungeonMaster.PickStartLocation(LevelManager.Level);
            BigBoss.DungeonMaster.SpawnCreature(new Point(loc.x, loc.y), "giant_spider");
        }
        */

        Debug.Log("Running GUI initialize.");
        BigBoss.Gooey.RegenInventoryGUI();
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