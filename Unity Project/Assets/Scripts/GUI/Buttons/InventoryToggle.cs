//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Very basic script that will activate or deactivate an object (and all of its children) when clicked.
/// </summary>

public class InventoryToggle : MonoBehaviour
{
    public InventoryMenu invMenu;
    public ItemMenu itemMenu;

    void OnClick()
    {
        /*
        bool newState = invMenu.gameObject.activeSelf;

        if (!newState)
        {
            NGUITools.SetActive(invMenu.gameObject, true);
        }
        else
        {
            NGUITools.SetActive(invMenu.gameObject, false);
            NGUITools.SetActive(itemMenu.gameObject, false);
        }
         */
        if (BigBoss.Gooey.inventory.isActive)
        {
            BigBoss.Gooey.inventory.Close();
        }
        else
        {
            BigBoss.Gooey.inventory.Open();
        }
    }
}