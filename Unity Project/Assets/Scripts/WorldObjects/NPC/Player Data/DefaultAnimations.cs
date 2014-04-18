using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DefaultAnimations : WeaponAnimations
{
    void Start()
    {
        Attack1 = "Base Layer.attack 1";
        Attack2 = "Base Layer.attack 2";
        Attack3 = "Base Layer.attack 3";
        Move = "";
    }
}