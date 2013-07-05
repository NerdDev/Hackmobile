using System;
using System.Collections.Generic;

public class NPCEquipment
{
    List<EquipSlot>[] equipSlots = new List<EquipSlot>[(int)EquipTypes.LAST];

    public NPCEquipment(NPCBodyParts bp)
    {
        equipSlots.Initialize();
        equipSlots[(int)EquipTypes.BODY].Add(new EquipSlot());
        equipSlots[(int)EquipTypes.SHIRT].Add(new EquipSlot());
        if (bp.Arms > 0)
        {
            for (int i = 0; i < bp.Arms; i++)
            {
                equipSlots[(int) EquipTypes.RING].Add(new EquipSlot());
                equipSlots[(int) EquipTypes.HAND].Add(new EquipSlot());
            }
        }
        if (bp.Legs > 0)
        {
            if (bp.Legs == 2)
            {
                equipSlots[(int) EquipTypes.LEGS].Add(new EquipSlot());
            }
            if (bp.Legs % 2 == 0 && bp.Legs >= 2) {
                for (int i = 2; i <= bp.Legs; i += 2)
                {
                    equipSlots[(int) EquipTypes.FEET].Add(new EquipSlot());
                }
            }
        }
        if (bp.Heads > 0)
        {
            for (int i = 0; i < bp.Heads; i++)
            {
                equipSlots[(int) EquipTypes.HEAD].Add(new EquipSlot());
                equipSlots[(int) EquipTypes.NECK].Add(new EquipSlot());
            }
        }
    }

    //Public accessors

    public bool isFreeSlot(EquipTypes et)
    {
        switch (et)
        {
            case EquipTypes.LEFT_RING:
                if (equipSlots[(int)EquipTypes.RING].Count == 2) //if the count isn't 2, they have an odd number of hands, so left/right makes no sense
                {
                    return equipSlots[(int) EquipTypes.RING][0].isFree();
                }
                else
                {
                    return false;
                }
            case EquipTypes.RIGHT_RING:
                if (equipSlots[(int)EquipTypes.RING].Count == 2) //if the count isn't 2, they have an odd number of hands, so left/right makes no sense
                {
                    return equipSlots[(int)EquipTypes.RING][1].isFree();
                }
                else
                {
                    return false;
                }
            default:
                return isFree(equipSlots[(int)et]); //this returns an ArrayOutOfBounds if EquipTypes.NONE is passed
        }

    }

    public bool equipItem(Item i)
    {
        if (isFreeSlot(i.EquipType))
        {
            EquipSlot es = getFreeSlot(i.EquipType);
            es.equipItem(i);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool removeItem(Item i)
    {
        EquipTypes et = i.EquipType;
        switch (et)
        {
            case EquipTypes.LEFT_RING:
                if (equipSlots[(int)EquipTypes.RING].Count == 2) //if the count isn't 2, they have an odd number of hands, so left/right makes no sense
                {
                    return equipSlots[(int)EquipTypes.RING][0].removeItem(i);
                }
                else
                {
                    return false;
                }
            case EquipTypes.RIGHT_RING:
                if (equipSlots[(int)EquipTypes.RING].Count == 2) //if the count isn't 2, they have an odd number of hands, so left/right makes no sense
                {
                    return equipSlots[(int)EquipTypes.RING][1].removeItem(i);
                }
                else
                {
                    return false;
                }
            default:
                return removeItem(i, equipSlots[(int)et]);
        }
    }

    /// Private functions

    private EquipSlot getFreeSlot(EquipTypes et)
    {
        switch (et)
        {
            case EquipTypes.LEFT_RING:
                if (equipSlots[(int)EquipTypes.RING].Count == 2 && equipSlots[(int)EquipTypes.RING][0].isFree()) //if the count isn't 2, they have an odd number of hands, so left/right makes no sense
                {
                    return equipSlots[(int)EquipTypes.RING][0];
                }
                else
                {
                    return null;
                }
            case EquipTypes.RIGHT_RING:
                if (equipSlots[(int)EquipTypes.RING].Count == 2 && equipSlots[(int)EquipTypes.RING][1].isFree()) //if the count isn't 2, they have an odd number of hands, so left/right makes no sense
                {
                    return equipSlots[(int)EquipTypes.RING][1];
                }
                else
                {
                    return null;
                }
            default:
                return getEmptySlot(equipSlots[(int)et]);
        }
    }

    /**
     * Only call this AFTER checking isFree(List<EquipSlot> list)
     *  to prevent any null exceptions.
     */
    private EquipSlot getEmptySlot(List<EquipSlot> list)
    {
        foreach (EquipSlot e in list)
        {
            if (e.isFree())
            {
                return e;
            }
        }
        return null;
    }

    private bool isFree(List<EquipSlot> list)
    {
        foreach (EquipSlot e in list)
        {
            if (e.isFree())
            {
                return true;
            }
        }
        return false;
    }

    private bool isFree(EquipSlot es)
    {
        if (es != null)
        {
            return es.isFree();
        }
        return false;
    }

    private bool removeItem(Item i, List<EquipSlot> boots)
    {
        foreach (EquipSlot es in boots)
        {
            if (es.removeItem(i))
            {
                return true;
            }
        }
        return false;
    }

    internal class EquipSlot
    {
        Item equipped;

        public EquipSlot()
        {
            equipped = null;
        }

        public bool isFree()
        {
            return (equipped == null);
        }

        public void equipItem(Item i) {
            this.equipped = i;
            i.itemFlags[ItemFlags.IS_EQUIPPED] = true;
        }

        public void removeItem() {
            this.equipped.itemFlags[ItemFlags.IS_EQUIPPED] = false;
            this.equipped = null;
        }

        public bool removeItem(Item i)
        {
            if (i.Equals(equipped))
            {
                removeItem();
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public void setNull()
    {
        for (int i = 0; i < equipSlots.Length; i++)
        {
            if (equipSlots[i] != null)
            {
                equipSlots[i] = null;
            }
        }
    }
}