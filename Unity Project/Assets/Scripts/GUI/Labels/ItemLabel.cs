using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Label")]
public class ItemLabel : GUIElement
{
    internal Action<Item, Item> registerAction;
    internal UILabel label;

    public virtual void Start()
    {
        if (label == null)
        {
            label = GetComponent<UILabel>();
            if (label == null)
            {
                label = gameObject.AddComponent<UILabel>();
            }
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        ItemMenu menu = parent is ItemMenu ? parent as ItemMenu : null;
        if (menu != null && registerAction != null && !menu.item.Registered(this))
        {
            menu.item.Register(this, registerAction);
        }
    }
}
