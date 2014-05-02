using System;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : IXmlParsable
{
    internal Dictionary<EquipType, EquipSlot> equipSlots = new Dictionary<EquipType, EquipSlot>();
    internal Dictionary<EquipType, Item> Default = new Dictionary<EquipType, Item>();
    internal HashSet<EquipType> filterSlots = new HashSet<EquipType>();

    internal HashSet<Item> EquippedItems = new HashSet<Item>();

    Animator anim;
    BoneStructure Bones;
    public WeaponAnimations WeaponAnims = WeaponAnimations.Default();

    public Equipment()
    {
    }

    public void AddAnimator(Animator animator)
    {
        anim = animator;
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

    public bool isFreeSlot(EquipType et, Item item)
    {
        if (filter(et)) return false;
        if (!equipSlots.ContainsKey(et))
        {
            equipSlots.Add(et, new EquipSlot());
        }
        return isFree(et, item);
    }

    public bool slotIsEmpty(EquipType et)
    {
        if (!equipSlots.ContainsKey(et))
        {
            return false;
        }
        return isEmpty(et);
    }

    public bool DefaultItemIsEquipped(EquipType et)
    {
        if (Default.ContainsKey(et) && Default[et].itemFlags[ItemFlags.IS_EQUIPPED]) return true;
        return false;
    }

    public bool equipItem(Item i, BoneStructure bones)
    {
        this.Bones = bones; //this refreshes bones
        if (i.itemFlags[ItemFlags.DEFAULT])
        {
            return defaultEquip(i, bones);
        }
        return normalEquip(i, bones);
    }

    private bool defaultEquip(Item i, BoneStructure bones)
    {
        EquipType et = i.stats.EquipType;
        if (slotIsEmpty(et)) //slot is empty, equip the default item normally
        {
            if (Default.ContainsKey(et)) Default.Remove(et); //get it on the list
            Default.Add(et, i);
            return equipIntoSlot(i, bones, et); //equip it
        }
        if (DefaultItemIsEquipped(et)) //default item is equipped but slot is not empty
        {
            removeItem(Default[et]); //remove it
            Default.Remove(et); //get it on the list
            Default.Add(et, i);
            return equipIntoSlot(i, bones, et); //then equip the new one
        }
        //default item is not equipped but slot is not empty
        Default.Remove(et);
        Default.Add(et, i); //don't equip it
        return false;
    }

    private bool normalEquip(Item i, BoneStructure bones)
    {
        EquipType et = i.stats.EquipType;
        if (DefaultItemIsEquipped(et)) removeItem(Default[et]);
        if (isFreeSlot(et, i))
        {
            return equipIntoSlot(i, bones, et);
        }
        else
        {
            if (removeItems(et))
            {
                return equipIntoSlot(i, bones, et);
            }
        }
        return false;
    }

    private bool equipIntoSlot(Item i, BoneStructure bones, EquipType et)
    {
        if (!equipSlots.ContainsKey(et)) return false;
        EquipSlot es = equipSlots[et];
        es.equipItem(i);
        EquippedItems.Add(i);
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

    public bool removeItems(EquipType type)
    {
        if (equipSlots.ContainsKey(type))
        {
            EquipSlot slot = equipSlots[type];
            List<Item> items = slot.GetEquipped();
            foreach (Item item in items)
            {
                removeItem(item);
            }
            return true;
        }
        return false;
    }

    public bool removeItem(Item i)
    {
        EquipType et = i.stats.EquipType;
        if (equipSlots.ContainsKey(et)) //if there's no slot, there's no item
        {
            if (equipSlots[et].removeItem(i))
            {
                i.Unwrap();
                EquippedItems.Remove(i);
                if (slotIsEmpty(et))
                {
                    if (!i.itemFlags[ItemFlags.DEFAULT] && Default.ContainsKey(et)) equipIntoSlot(Default[et], Bones, et);
                }
                return true;
            }
        }
        return false;
    }

    public bool isFree(EquipType et, Item item)
    {
        return GetSlot(et).canEquip(item);
    }

    public bool isEmpty(EquipType et)
    {
        return GetSlot(et).isEmpty();
    }

    private EquipSlot GetSlot(EquipType type)
    {
        if (equipSlots[type] == null)
        {
            equipSlots[type] = new EquipSlot();
        }
        return equipSlots[type];
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

        public void equipItem(Item i)
        {
            if (canEquip(i))
            {
                equipped.Add(i);
                if (!i.itemFlags[ItemFlags.DEFAULT]) numSlots -= i.stats.NumberOfSlots;
                i.itemFlags[ItemFlags.IS_EQUIPPED] = true;
            }
        }

        public bool removeItem(Item i)
        {
            if (equipped.Contains(i))
            {
                if (!i.itemFlags[ItemFlags.DEFAULT]) numSlots += i.stats.NumberOfSlots;
                equipped.Remove(i);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void removeItems()
        {
            foreach (Item i in equipped)
            {
                if (!i.itemFlags[ItemFlags.DEFAULT]) numSlots += i.stats.NumberOfSlots;
            }
            equipped.Clear();
        }

        public bool canEquip(Item i)
        {
            if (numSlots >= i.stats.NumberOfSlots)
            {
                return true;
            }
            return false;
        }

        public bool isEmpty()
        {
            return equipped.Count == 0;
        }

        public List<Item> GetEquipped()
        {
            return new List<Item>(equipped);
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
