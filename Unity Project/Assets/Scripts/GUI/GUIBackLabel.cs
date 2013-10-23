using System;
using UnityEngine;

public class GUIBackLabel : MonoBehaviour
{
    void OnClick()
    {
        BigBoss.Gooey.category = "";
        BigBoss.Gooey.categoryDisplay = false;
        BigBoss.Gooey.RegenInventoryGUI();
    }
}