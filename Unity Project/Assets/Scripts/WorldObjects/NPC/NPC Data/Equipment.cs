using System;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : IXmlParsable
{
    internal Dictionary<EquipType, EquipSlot> equipSlots = new Dictionary<EquipType, EquipSlot>();
    internal HashSet<EquipType> filterSlots = new HashSet<EquipType>();

    public WeaponAnimations WeaponAnims = WeaponAnimations.Default();

    public Equipment()
    {
    }

    public void AddSlot(EquipType et, int slots = 1)
    {
        if (!equipSlots.ContainsKey(et))
        {
            equipSlots.Add(et, new EquipSlot(slots));
            filterSlots.Add(et);
        }
    }

    //Public accessors

    public bool isFreeSlot(EquipType et)
    {
        if (filter(et)) return false;
        if (!equipSlots.ContainsKey(et))
        {
            equipSlots.Add(et, new EquipSlot());
        }
        return isFree(et);
    }

    public bool equipItem(Item i, BoneStructure bones)
    {
        EquipType et = i.stats.EquipType;
        if (isFreeSlot(et))
        {
            EquipSlot es = equipSlots[et];
            es.equipItem(i);
            if (bones != null)
            {
                //wrap object to NPC's bone
                GameObject item = BigBoss.Objects.Items.WrapEquipment(i, bones).gameObject;
                //grab animations if it's a weapon
                WeaponAnimations temp = item.GetComponent<WeaponAnimations>();
                if (temp != null) WeaponAnims = temp;
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool removeItem(Item i, Animator anim)
    {
        EquipType et = i.stats.EquipType;
        if (!isFreeSlot(et)) //if it's free, there's no item for that type
        {
            if (equipSlots[et].removeItem(i))
            {
                i.Unwrap();
                if (WeaponAnims.Move != "") anim.SetBool(WeaponAnims.Move, false);
                WeaponAnims = WeaponAnimations.Default();
            }
        }
        return false;
    }

    public bool isFree(EquipType et)
    {
        if (equipSlots[et] == null)
        {
            equipSlots[et] = new EquipSlot();
        }
        return equipSlots[et].isFree();
    }

    public List<Item> GetWeapons()
    {
        if (equipSlots.ContainsKey(EquipType.HAND))
        {
            return equipSlots[EquipType.HAND].GetEquipped();
        }
        return new List<Item>();
    }

    private bool filter(EquipType et)
    {
        if (filterSlots.Contains(et))
        {
            return false;
        }
        return true;
    }

    internal class EquipSlot
    {
        List<Item> equipped = new List<Item>();
        int numSlots;

        public EquipSlot()
        {
            numSlots = 1;
        }

        public EquipSlot(int slots)
        {
            numSlots = slots;
        }

        public bool isFree()
        {
            return (equipped.Count == 0);
        }

        public void equipItem(Item i)
        {
            if (canEquip(i))
            {
                equipped.Add(i);
                numSlots -= i.stats.NumberOfSlots;
                i.itemFlags[ItemFlags.IS_EQUIPPED] = true;
            }
        }

        public bool removeItem(Item i)
        {
            if (equipped.Contains(i))
            {
                numSlots += i.stats.NumberOfSlots;
                equipped.Remove(i);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool canEquip(Item i)
        {
            if (numSlots >= i.stats.NumberOfSlots)
            {
                return true;
            }
            return false;
        }

        public List<Item> GetEquipped()
        {
            return equipped;
        }
    }

    public void ParseXML(XMLNode x)
    {
        foreach (XMLNode node in x.SelectList("equiptype"))
        {
            EquipType et = node.SelectEnum<EquipType>("type");
            int slots = node.SelectInt("slots", 1);
            equipSlots.Add(et, new EquipSlot(slots));
            filterSlots.Add(et);
        }
    }
}
