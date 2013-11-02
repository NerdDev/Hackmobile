using System;
using UnityEngine;

public class GUIUseItem : GUILabel
{
    public ItemList item;

    void OnClick()
    {
        if (item.Count > 0)
        {
            BigBoss.Player.useItem(item[item.Count - 1]);
            BigBoss.Gooey.RegenItemInfoGUI(item);
        }
    }

    void OnDoubleClick()
    {

    }
}