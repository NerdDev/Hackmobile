using System;
using System.Collections.Generic;
using UnityEngine;

public class GroundGrid : ScrollingGrid
{
    bool categoryDisplay = true;
    string category = "";
    bool displayItem;
    GroundMenu chestParent;

    public override void Initialize()
    {
        base.Initialize();
        if (parent is GroundMenu) chestParent = parent as GroundMenu;
        else return; //not under an ItemMenu, which means there's no item to access.

        if (parent.displayMenu && chestParent.chest != null)
        {
            Inventory inv = chestParent.chest.Location.inventory;
            this.gameObject.SetActive(true);
            this.Clear();
            foreach (InventoryCategory ic in inv.Values)
            {
                foreach (Item item in ic.Values)
                {
                    CreateItemButton(item);
                }
            }
            CreateCloseLabel();
            this.Reposition();
        }
        else
        {
            this.Clear();
            BigBoss.Gooey.itemMenu.Close();
        }
    }

    void CreateItemButton(Item item)
    {
        string buttonText;
        if (item.Count > 1)
        {
            buttonText = item.Name + " (" + item.Count + ")";
        }
        else { buttonText = item.Name; }
        GUIButton itemButton = BigBoss.Gooey.CreateObjectButton(item, this, item.Name, buttonText);
        itemButton.OnSingleClick = new Action(() =>
        {
            if ((itemButton.refObject as Item).Count > 0)
            {
                displayItem = true;
                BigBoss.Gooey.itemMenu.Open(itemButton.refObject as Item);
            }
        });
    }

    private void CreateCloseLabel()
    {
        GUIButton button = BigBoss.Gooey.CreateButton(this, "CloseButton", "Close");
        button.OnSingleClick = new Action(() =>
        {
            BigBoss.Gooey.category = "";
            BigBoss.Gooey.categoryDisplay = false;
            BigBoss.Gooey.ground.Close();
            BigBoss.Gooey.displayItem = false;
            BigBoss.Gooey.itemMenu.Close();
        });
    }
}
