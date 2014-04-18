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
        Item ii = BigBoss.Objects.Items.Instantiate("Infiltrator's Sword");
        BigBoss.Player.addToInventory(ii);

        Item food = BigBoss.Objects.Items.Instantiate("Spoiled Bread");
        BigBoss.Player.addToInventory(food, 5);

        Item potion = BigBoss.Objects.Items.Instantiate("Health Potion");
        BigBoss.Player.addToInventory(potion, 3);

        Item potion3 = BigBoss.Objects.Items.Instantiate("Cure Poison");
        BigBoss.Player.addToInventory(potion3, 3);

        Item potion4 = BigBoss.Objects.Items.Instantiate("Levitation Potion");
        BigBoss.Player.addToInventory(potion4, 3);

        Item potion5 = BigBoss.Objects.Items.Instantiate("Blinding Potion");
        BigBoss.Player.addToInventory(potion5, 3);
    }
}

class A
{
    public string a1;
    public string a2;
    public A(string a, string b)
    {
        a1 = a;
        a2 = b;
    }
}
