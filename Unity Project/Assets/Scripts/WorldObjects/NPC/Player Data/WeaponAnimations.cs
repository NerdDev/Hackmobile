using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WeaponAnimations : MonoBehaviour
{
    static DefaultAnimations DefaultAnims;

    public string Attack1 = "";
    public string Attack2 = "";
    public string Attack3 = "";
    public string Idle = "";
    public string Move = "";

    public static DefaultAnimations Default()
    {
        if (DefaultAnims == null)
        {
            DefaultAnims = new GameObject("DefaultAnimationsObj", typeof(DefaultAnimations)).GetComponent<DefaultAnimations>();
            DefaultAnims.transform.parent = BigBoss.PlayerInfo.transform;
            GameObject.DontDestroyOnLoad(DefaultAnims);
            DefaultAnims.transform.Reset();
        }
        return DefaultAnims;
    }

    public void CopyInto(WeaponAnimations animations)
    {
        Attack1 = animations.Attack1;
        Attack2 = animations.Attack2;
        Attack3 = animations.Attack3;
        Idle = animations.Idle;
        Move = animations.Move;
    }
}