using System;
using UnityEngine;

public class GUIInventoryCategory : GUILabel
{
    public InventoryCategory category;

    void OnClick()
    {
        BigBoss.Gooey.categoryDisplay = true;
        BigBoss.Gooey.category = category.id;
        BigBoss.Gooey.RegenInventoryGUI();
    }
}