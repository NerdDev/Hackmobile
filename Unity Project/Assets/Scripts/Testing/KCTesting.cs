using System;
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
        A a = new A(1, 2, 3, "fire", "ice");
        A b = new A(1, 2, 4, "fire", "ice2");
        A c = new A(2, 3, 4, "fire1", "ice1");

        Debug.Log(a.GetHashCode());
        Debug.Log(b.GetHashCode());
        Debug.Log(c.GetHashCode());

        Debug.Log(a.GetHash());
        Debug.Log(b.GetHash());
        Debug.Log(c.GetHash());

        A d = a.Copy();
        Debug.Log("d hash: " + d.GetHash());
         * */
        //Input checks
        BigBoss.PlayerInput.allowKeyboardInput = true;
        BigBoss.PlayerInput.allowMouseInput = true;
        BigBoss.PlayerInput.allowPlayerInput = true;

        //adds some items to the Player to test with
        Item ii = BigBoss.Objects.Items.Instantiate("sword1");
        BigBoss.Player.addToInventory(ii);

        Item food = BigBoss.Objects.Items.Instantiate("Spoiled Bread");
        BigBoss.Player.addToInventory(food, 5);

        Item potion = BigBoss.Objects.Items.Instantiate("Health Potion");
        //potion.Icon = "icon1";
       // Debug.Log(potion.onEaten.GetHash());
        BigBoss.Player.addToInventory(potion, 3);

        Item potion3 = BigBoss.Objects.Items.Instantiate("Cure Poison");
        //potion3.Icon = "icon3";
        //Debug.Log(potion3.onEaten.GetHash());
        BigBoss.Player.addToInventory(potion3, 3);

        Item potion4 = BigBoss.Objects.Items.Instantiate("Levitation Potion");
        //potion4.Icon = "icon4";
        //Debug.Log(potion4.onEaten.GetHash());
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
