using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Label")]
public class ArmorValueLabel : ItemLabel
{
    public override void Start()
    {
        base.Start();
        registerAction = new Action<Item, Item>((oldItem, newItem) =>
        {
            label.text = "Armor: " + 0;
        });
    }

    public override void Initialize()
    {
        base.Initialize();
    }
}
