using System;
using UnityEngine;

public class GUIItem : GUILabel
{
    public ItemList item;

    void OnClick()
    {
        if (item.Count > 0)
        {
            BigBoss.Gooey.displayItem = true;
            BigBoss.Gooey.RegenItemInfoGUI(item);
        }
    }

    void OnDoubleClick()
    {

    }
}