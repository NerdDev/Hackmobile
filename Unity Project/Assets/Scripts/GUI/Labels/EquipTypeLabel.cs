using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/Label")]
public class EquipTypeLabel : ItemLabel
{
    public override void Start()
    {
        base.Start();
        registerAction = new Action<Item, Item>((oldItem, newItem) =>
        {
            label.text = "Equip Type: " + newItem.stats.EquipType.ToString();
        });
    }

    public override void Initialize()
    {
        base.Initialize();
    }
}
