using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TwoHandedSword : WeaponAnimations
{
    void Start()
    {
        Attack1 = "Base Layer.TwoHandedAttack1";
        Attack2 = "Base Layer.TwoHandedAttack2";
        Attack3 = "Base Layer.TwoHandedAttack3";
        Move = "TwoHanded";
    }
}