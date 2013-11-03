using System;
using UnityEngine;

public class GUIBackLabel : GUILabel
{
    void OnClick()
    {
        BigBoss.Gooey.category = "";
        BigBoss.Gooey.categoryDisplay = false;
        BigBoss.Gooey.RegenInventoryGUI();

        BigBoss.Gooey.displayItem = false;
        BigBoss.Gooey.RegenItemInfoGUI(null);
    }
}