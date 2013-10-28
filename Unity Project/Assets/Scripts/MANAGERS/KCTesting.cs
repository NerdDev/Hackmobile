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
        //Places the player close to some stairs instead of the random position
        BigBoss.Player.transform.position = new Vector3(83f, -.5f, 58f);

        //adds some items to the Player to test with
        Item ii = BigBoss.WorldObject.CreateItem("sword1");
        BigBoss.Player.addToInventory(ii);
        BigBoss.Player.equipItem(ii);

        Item food = BigBoss.WorldObject.CreateItem("spoiled bread");
        BigBoss.Player.addToInventory(food, 5);

        Item potion = BigBoss.WorldObject.CreateItem("health potion");
        BigBoss.Player.addToInventory(potion, 3);
    }
}