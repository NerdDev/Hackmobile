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
        //Input checks
        BigBoss.PlayerInput.allowKeyboardInput = true;
        BigBoss.PlayerInput.allowMouseInput = true;
        BigBoss.PlayerInput.allowPlayerInput = true;

        //adds some items to the Player to test with
        Item ii = BigBoss.Objects.Items.Instantiate("sword1");
        BigBoss.Player.addToInventory(ii);

        Item food = BigBoss.Objects.Items.Instantiate("spoiled bread");
        BigBoss.Player.addToInventory(food, 5);

        Item potion = BigBoss.Objects.Items.Instantiate("health potion");
        BigBoss.Player.addToInventory(potion, 3);
    }
}
