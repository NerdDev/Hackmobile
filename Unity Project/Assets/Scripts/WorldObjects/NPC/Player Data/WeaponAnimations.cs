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
}