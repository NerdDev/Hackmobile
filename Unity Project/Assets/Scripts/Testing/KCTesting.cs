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
        Dictionary<int, Item> dict = new Dictionary<int, Item>();

        //Input checks
        BigBoss.PlayerInput.allowKeyboardInput = true;
        BigBoss.PlayerInput.allowMouseInput = true;
        BigBoss.PlayerInput.allowPlayerInput = true;

        //adds some items to the Player to test with
        Item ii = BigBoss.Objects.Items.Instantiate("sword1");
        BigBoss.Player.addToInventory(ii);

        Item food = BigBoss.Objects.Items.Instantiate("Spoiled Bread");
        dict.Add(4, food);
        BigBoss.Player.addToInventory(food, 5);

        Dictionary<int, Item> dict2 = dict.Copy();
        Debug.Log(dict[4].GetHashCode());
        Debug.Log(dict2[4].GetHashCode());

        Item potion = BigBoss.Objects.Items.Instantiate("Health Potion");
        Debug.Log(potion.GetHash());
        BigBoss.Player.addToInventory(potion, 3);

        Item potion3 = BigBoss.Objects.Items.Instantiate("Cure Poison");
        Debug.Log(potion3.GetHash());
        BigBoss.Player.addToInventory(potion3, 3);

        Item potion4 = BigBoss.Objects.Items.Instantiate("Levitation Potion");
        Debug.Log(potion4.GetHash());
        BigBoss.Player.addToInventory(potion4, 3);

        Item potion5 = BigBoss.Objects.Items.Instantiate("Blinding Potion");
        BigBoss.Player.addToInventory(potion5, 3);
    }

    class A
    {
        public int a, b;
        internal int c;
        public string str1;
        private string str2;

        public A(int num1, int num2, int num3, string s1, string s2)
        {
            this.a = num1;
            this.b = num2;
            this.c = num3;
            this.str1 = s1;
            this.str2 = s2;
        }
    }
}
