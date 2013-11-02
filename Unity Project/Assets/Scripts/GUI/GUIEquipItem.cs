using System;
using UnityEngine;

public class GUIEquipItem : GUILabel
{
    public ItemList item;

    void OnClick()
    {
        if (item.Count > 0)
        {
            BigBoss.Player.equipItem(item[item.Count - 1]);
            BigBoss.Gooey.RegenItemInfoGUI(item);
        }
    }

    void OnDoubleClick()
    {

    }
}