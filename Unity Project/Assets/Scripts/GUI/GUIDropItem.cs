using System;
using UnityEngine;

public class GUIDropItem : GUILabel
{
    public ItemList item;

    void OnClick()
    {
        if (item.Count > 0)
        {
            //WOO WE HAVE NOTHING TO HANDLE ITEMS IN THE GAME WORLD
        }
    }

    void OnDoubleClick()
    {

    }
}