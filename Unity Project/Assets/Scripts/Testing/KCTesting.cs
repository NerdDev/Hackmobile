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
        BigBoss.PlayerInput.InputSetting[InputSettings.KEYBOARD_INPUT] = true;
        BigBoss.PlayerInput.InputSetting[InputSettings.TOUCH_INPUT] = true;
        BigBoss.PlayerInput.InputSetting[InputSettings.PLAYER_INPUT] = true;
        BigBoss.PlayerInput.InputSetting[InputSettings.DEFAULT_INPUT] = true;

        //adds some items to the Player to test with
        EquipItemToPlayer("NoseA");
        EquipItemToPlayer("EarsA");
        EquipItemToPlayer("HairC");
        EquipItemToPlayer("BootsDefault");
        EquipItemToPlayer("Body");

        GiveItemToPlayer("Leather Armor");
        GiveItemToPlayer("Leather Skirt");
        GiveItemToPlayer("Leather Boots");
        GiveItemToPlayer("Elven Armor");
        GiveItemToPlayer("Elven Skirt");
        GiveItemToPlayer("Elven Boots");
        GiveItemToPlayer("Elven Helmet");
        GiveItemToPlayer("Mage's Legs");
        GiveItemToPlayer("Mage's Boots");
        GiveItemToPlayer("Mage's Outfit");

        GiveItemToPlayer("Spoiled Bread");
        GiveItemToPlayer("Health Potion");
        GiveItemToPlayer("Cure Poison");
        GiveItemToPlayer("Levitation Potion");
        GiveItemToPlayer("Blinding Potion");
    }

    private void GiveItemToPlayer(string item)
    {
        Item i = BigBoss.Objects.Items.Instantiate(item);
        BigBoss.Player.addToInventory(i);
    }

    private void EquipItemToPlayer(string item)
    {
        Item i = BigBoss.Objects.Items.Instantiate(item);
        BigBoss.Player.addToInventory(i);
        BigBoss.Player.equipItem(i);
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
