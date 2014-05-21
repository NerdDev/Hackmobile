using System;
using UnityEngine;

public class ItemButton : GUIButton
{
    public Item i;

    public override void Initialize()
    {
        base.Initialize();
        ItemMenu menu = parent is ItemMenu ? parent as ItemMenu : null;
        if (menu != null && !menu.item.Registered(this))
        {
            i = menu.item.Get();
            menu.item.Register(this, new Action<Item, Item>((oldItem, newItem) =>
            {
                i = newItem;
            }));
        }
    }
}