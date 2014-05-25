using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGrid : ScrollingGrid
{
    bool categoryDisplay = false;
    string category = "";
    bool displayItem;

    internal Inventory inv;
    void Start()
    {
        inv = BigBoss.Player.Inventory;
        inv.hasGUIUpdates = true;
        inv.invGrid = this;
    }

    public override void Initialize()
    {
        base.Initialize();
        if (this.parent.displayMenu && inv != null)
        {
            this.gameObject.SetActive(true);
            this.Clear();
            if (!categoryDisplay)
            {
                foreach (InventoryCategory ic in inv.Values)
                {
                    CreateCategoryButton(ic);
                }
            }
            else
            {
                InventoryCategory ic;
                if (inv.TryGetValue(category, out ic))
                {
                    foreach (Item item in ic.Values)
                    {
                        CreateItemButton(item);
                    }
                    CreateBackLabel();
                }
                else
                {
                    Close();
                    return;
                }
            }
            this.Reposition();
        }
        else
        {
            this.Clear();
        }
    }

    public override void Clear()
    {
        base.Clear();
        //BigBoss.Gooey.CloseItemMenu();
    }

    public void Close()
    {
        category = "";
        categoryDisplay = false;
        displayItem = false;
        Initialize();
        BigBoss.Gooey.itemMenu.Close();
    }

    void CreateCategoryButton(InventoryCategory ic)
    {
        GUIButton categoryButton = BigBoss.Gooey.CreateObjectButton(ic, this, ic.id);
        categoryButton.OnSingleClick = new Action(() =>
        {
            categoryDisplay = true;
            category = (categoryButton.refObject as InventoryCategory).id;
            Initialize();
        });
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

    void CreateBackLabel()
    {
        GUIButton button = BigBoss.Gooey.CreateButton(this, "BackButton", "Back");
        button.OnSingleClick = new Action(() =>
        {
            Close();
        });
    }
}
